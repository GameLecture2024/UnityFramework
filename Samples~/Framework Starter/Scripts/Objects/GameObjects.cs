using UnityEngine;

namespace GameLecture.Framework.Samples
{
    public class Player : ObjectBase { }
    public class Monster : ObjectBase { }
    public class Npc : ObjectBase { }

    public sealed class GameObjectManager : Singleton<GameObjectManager>
    {
        private Transform _playerRoot;
        private Transform _monsterRoot;
        private Transform _npcRoot;

        public Player SpawnPlayer(string prefab = "Player", bool pooling = false) =>
            ObjectManager.Instance.Spawn<Player>(prefab,
                FrameworkUtils.GetOrCreateRoot(ref _playerRoot, "@Players"), pooling);

        public Monster SpawnMonster(string prefab = "Monster", bool pooling = false) =>
            ObjectManager.Instance.Spawn<Monster>(prefab,
                FrameworkUtils.GetOrCreateRoot(ref _monsterRoot, "@Monsters"), pooling);

        public Npc SpawnNpc(string prefab = "Npc", bool pooling = false) =>
            ObjectManager.Instance.Spawn<Npc>(prefab,
                FrameworkUtils.GetOrCreateRoot(ref _npcRoot, "@NPCs"), pooling);
    }
}
