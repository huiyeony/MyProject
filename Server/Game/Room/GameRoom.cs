using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Object;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer
    {
        object _lock = new object();
        public int RoomId { get; set; } //룸 고유 번호

        Dictionary<int, Player> _players = new Dictionary<int, Player>();//유저 리스트 
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();//몬스터 리스트
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();//발사체 리스트 
        Dictionary<int, Resource> _resources = new Dictionary<int, Resource>();//자원 리스트

        public Map Map { get; private set; } = new Map();//맵에 대한 정보를 로드할 때마다 갈아 끼움 !!

        //zone
        //ㅁ ㅁ ㅁ 
        //ㅁ ㅁ ㅁ 
        //ㅁ ㅁ ㅁ
        public const int ViewCells = 20;
        public Zone[,] Zones { get; private set; }
        public int ZoneCells { get; private set; }

        //cellPos -> x축 , y축
        //zone,pos -> 행렬

        //ㅁ ㅁ ㅁ
        //ㅁ ㅁ ㅁ 
        //ㅁ ㅁ ㅁ

        public Zone GetZone(Vector2Int cellPos)
        {
            int indexX = (cellPos.x - Map.MinX )/ ZoneCells;
            int indexY = (Map.MaxY - cellPos.y )/ ZoneCells;

            if (indexY < 0 || indexY >= Zones.GetLength(0))
                return null;
            if (indexX < 0 || indexX >= Zones.GetLength(1))
                return null;
            
            return Zones[indexY, indexX];
        }
        public void Init(int mapId, int zoneCells)
        {
            Map.LoadMap(mapId);//맵 정보 갈아 끼우기

            //Zone
            ZoneCells = zoneCells;
            int countY = (Map.SizeY + zoneCells - 1) / ZoneCells;
            int countX = (Map.SizeX + zoneCells - 1) / ZoneCells;

            Zones = new Zone[countY, countX];//존의 개수 

            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    Zones[y, x] = new Zone(y, x);//존 생성 
                }
            }
        }
        public void Update()//발사체 위치 업데이트 
        {
            lock (_lock)
            {
                foreach (Monster m in _monsters.Values)
                {
                    m.Update();// 발사체 위치 업데이트 
                }
                foreach (Projectile p in _projectiles.Values)
                {
                    p.Update();
                }
                foreach(Resource r in _resources.Values)
                {
                    PushAfter(50, () => { r.Update(); });
                }
                
            }
        }

        Random _rand = new Random();
        public void EnterGame(GameObject newObject, bool randomPos)//게임 룸에 입장 
        {
            if (newObject == null) return;

            if (randomPos)
            {
                Vector2Int randSpawn = new Vector2Int();

                randSpawn.x = _rand.Next(Map.MinX, Map.MaxX + 1);
                randSpawn.y = _rand.Next(Map.MinY, Map.MaxY + 1);

                //단 아이템위치에는 갈 수 있음 !  
                if (Map.Find(randSpawn) == null)
                    newObject.CellPos = randSpawn;
            }
                
            //오브젝트 타입 
            GameObjectType type = ObjectManager.GetObjTypeById(newObject.Info.Id);

            lock (_lock)
            {
                Vector2Int cellPos = new Vector2Int();

                if (type == GameObjectType.Player)
                {
                    Player newPlayer = newObject as Player;
                    if (newPlayer == null)//잘못된 정보
                        return;


                    _players.Add(newPlayer.Info.Id, newPlayer);//목록에 추가
                    newPlayer.Room = this;//게임 방 저장 
                    
                    Map.ApplyMove(newPlayer, new Vector2Int(newPlayer.CellPos.x, newPlayer.CellPos.y));//갈 수 없는 구역에 추가 
                    //zone
                    GetZone(newPlayer.CellPos).Players.Add(newPlayer);
                    cellPos = newPlayer.CellPos;

                  

                    SEnterGame enterPkt = new SEnterGame();//뉴비에게 자신의 정보 알려주기
                    enterPkt.Info = newPlayer.Info;
                    enterPkt.TimeInfo = newPlayer.TimeInfo;


                    newPlayer.Session.Send(enterPkt);

                    newPlayer.Vision.Update();//여기서 스폰 & 디스폰 

                }
                else if (type == GameObjectType.Monster)//몬스터가 입장 
                {
                    Monster monster = newObject as Monster;

                    _monsters.Add(monster.Info.Id, monster);//사전에 추가
                    monster.Room = this;
                    Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));//갈 수 없는 구역에 추가 
                    //zone
                    GetZone(monster.CellPos).Monsters.Add(monster);
                    cellPos = monster.CellPos;
                }
                else if (type == GameObjectType.Projectile)//발사체가 입장 
                {
                    Projectile projectile = newObject as Projectile;
                    _projectiles.Add(projectile.Info.Id, projectile);//사전에 추가 
                    projectile.Room = this;

                    Map.ApplyMove(projectile, new Vector2Int(projectile.CellPos.x, projectile.CellPos.y), applyCollision: false);
                    //zone
                    GetZone(projectile.CellPos).Projectiles.Add(projectile);
                    cellPos = projectile.CellPos;
                }
                
                else if(type == GameObjectType.Resource)
                {
                    
                    Resource resource = newObject as Resource;
                    _resources.Add(resource.Info.Id, resource);
                    resource.Room = this ;
                    //아이템 자리 예외!
                    Map.ApplyMove(resource, new Vector2Int(resource.CellPos.x, resource.CellPos.y), applyCollision : false);
                    //zone 
                    GetZone(resource.CellPos).Resources.Add(resource);
                    cellPos = resource.CellPos;
                }

                SSpawn spawnPacket = new SSpawn();
                spawnPacket.Infos.Add(newObject.Info);
                Broadcast(cellPos, spawnPacket);

            }
        
            

        }
        public void LeaveGame(int objectId)//게임 룸에서 퇴장 + 위치 정보도 null 
        {
            GameObjectType type = ObjectManager.GetObjTypeById(objectId);
            GameObject go = null;
            lock (_lock)
            {
               Vector2Int cellPos = new Vector2Int();
               if(type == GameObjectType.Player)
               {

                    Player player = null;
                    //이 캐릭터를 리스트에서 삭제 

                    if (_players.Remove(objectId, out player) == false)
                        return;

                    
                    //갈수없는 지역에서  삭제
                    Map.ApplyLeave(player);


                    //나가기 전에 hp저장
                    player.OnLeave();
                    player.Room = null;
                    go = player;

                    //본인한테 정보 전송
                    {
                        SLeaveGame leavePacket = new SLeaveGame() { Id = player.Info.Id };
                        player.Session.Send(leavePacket);
                    }
               }
               else if(type == GameObjectType.Monster)
               {
                    Monster monster = null;
                    //리스트에서 삭제 
                    if (_monsters.Remove(objectId, out monster) == false)
                        return;

                    //갈수 없는 지역에서 삭제 
                    Map.ApplyLeave(monster);

                    go = monster;
                    monster.Room = null;
                }
               else if(type == GameObjectType.Projectile)
               {
                    Projectile projectile = null;
                    if (_projectiles.Remove(objectId, out projectile) == false)
                        return;
                    //갈수 없는 지역에서 삭제

                    Map.ApplyLeave(projectile, applyCollision: false);


                    projectile.Room = null;
                    go = projectile;
                }
                else if(type == GameObjectType.Resource)
                {
                    Resource resource = null;
                    if (_resources.Remove(objectId, out resource) == false)
                        return;

                    Map.ApplyLeave(resource, applyCollision: false);

                    resource.Room = null;
                    go = resource;
                }


                SDespawn despawnPacket = new SDespawn();
                despawnPacket.Infos.Add(go.Info);
                Broadcast(cellPos, despawnPacket);

            }
        }
    
         public void Broadcast(Vector2Int cellPos , IMessage packet)
         {
            lock (_lock)
            {

                
                // 주변 존에 있는 사람에게만 뿌리자!
                //todo : 시야각 안 
                List<Zone> zones = GetAdjacentZones(cellPos);
                foreach(Zone zone in zones)
                {
                    foreach (Player p in zone.Players)
                    {
                        int dx = cellPos.x - p.CellPos.x;
                        int dy = cellPos.y - p.CellPos.y;

                        if (Math.Abs(dx) > ViewCells) continue;
                        if (Math.Abs(dy) > ViewCells) continue;


                        p.Session.Send(packet);
                    }
                        
                }



            }
        }
        public List<Zone> GetAdjacentZones(Vector2Int cellPos, int cells = ViewCells )
        {
            HashSet<Zone> zones = new HashSet<Zone>();//중복 x

            int[] delta = new int[] { -cells, +cells }; 
            foreach (int dy in delta)
            {
                foreach (int dx in delta)
                {
                    Zone zone = GetZone(new Vector2Int(cellPos.x + dx, cellPos.y + dy));

                    if (zone != null)//범위가 넘어간 존은 제외함 
                        zones.Add(zone);
                }
            }

            return zones.ToList();
        }
        public void HandleMove(GameObject myObject,CMove movePacket)
        {
            lock (_lock)
            {
         
                PositionInfo movePosition = movePacket.PositionInfo;//클라에서 원하는 좌표 
                ObjectInfo info = myObject.Info;//현재 좌표 

                //todo : 검증
                Map.ApplyMove(myObject,new Vector2Int(movePosition.PosX,movePosition.PosY));


                //서버에 있는 정보 업데이트
                myObject.Info.PositionInfo = movePosition;

                //같은 방에 있는 사람들에게 뿌려줌
                SMove resMovePacket = new SMove();
                resMovePacket.Id = info.Id;
                resMovePacket.PositionInfo = info.PositionInfo;

                Broadcast(myObject.CellPos ,resMovePacket);
            }

        }
         public void HandleSkill(Player player,CSkill skillPacket)
         { 
            lock (_lock)
            {
                //todo :스킬 사용 가능 여부 판단 

                //Idle 상태에서만 스킬 사용
                ObjectInfo info = player.Info;
                if (info.PositionInfo.State != CreatureState.Idle)
                    return;

                //방 사람들에게 (누가), (어떤 스킬)을 썼는지 알려준다
                SSkill skillOkPacket = new SSkill() { SkillInfo = new SkillInfo() };
                skillOkPacket.Id = player.Info.Id;//플레이어 고유 아이디
                skillOkPacket.SkillInfo.SkillId = skillPacket.SkillInfo.SkillId;

                Broadcast(player.CellPos, skillOkPacket);

                //json 파일에서 스킬 데이터 있는지 확인 
                Skill skill = null;
                if (DataManager.SkillDict.TryGetValue(skillPacket.SkillInfo.SkillId, out skill) == false)
                    return;
                //플레이어가 스킬 씀 
                if (skill.skillType == SkillType.Unarmed)
                {

                    Vector2Int skillPos = player.GetFrontCellPos(info.PositionInfo.MoveDir);
                    GameObject go = Map.Find(skillPos);
                    //누군가 있음
                    //todo : 주먹 스킬 피격 판정
                    if (go != null)
                        go.OnDamaged(player, player.TotalAttack);//기본 공격력 & 획득한 무기 공격력++
                }
                else if(skill.skillType == SkillType.Arrow)
                {
                    //todo : Arrow
                    Arrow arrow = ObjectManager.Instance.Add<Arrow>();

                    //json 파일 
                    arrow.Skill = skill;
                    arrow.Owner = player;
                    arrow.Room = this;
                    arrow.Info.PositionInfo.State = CreatureState.Moving;//움직이는 중
                    arrow.Info.PositionInfo.MoveDir = info.PositionInfo.MoveDir;//내가 보는 방향 
                    arrow.CellPos = player.CellPos;//주인님과 같은 위치

                    //화살 스폰시 모두에게 알려주자 
                    arrow.Stat.Speed = skill.projectileInfo.speed;

                    EnterGame(arrow,false);//게임룸 입장 
                }
                    
            }


        }
         public Player Find(Func<GameObject,bool> condition)
         {
            foreach(Player p in _players.Values)
            {
                if (condition.Invoke(p))
                    return p;
            }
            return null;

        }
       

    }
}
