using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLecture.Framework.Samples
{
    [Serializable]
    public sealed class ItemData
    {
        public int id;
        public string name;
    }

    [Serializable]
    public sealed class ItemDataLoader : IDataLoader<int, ItemData>
    {
        public List<ItemData> items = new();
        public Dictionary<int, ItemData> MakeDictionary()
        {
            var result = new Dictionary<int, ItemData>();
            foreach (var item in items)
                result.Add(item.id, item);
            return result;
        }
        public bool Validate() => items.TrueForAll(item => item != null && item.id >= 0);
    }

    [Serializable]
    public sealed class LocalizedTextData
    {
        public string templateId;
        public string korean;
        public string english;
    }

    [Serializable]
    public sealed class LocalizedTextDataLoader : IDataLoader<string, LocalizedTextData>
    {
        public List<LocalizedTextData> texts = new();
        public Dictionary<string, LocalizedTextData> MakeDictionary()
        {
            var result = new Dictionary<string, LocalizedTextData>();
            foreach (var text in texts)
                result.Add(text.templateId, text);
            return result;
        }
        public bool Validate() => texts.TrueForAll(text =>
            text != null && !string.IsNullOrWhiteSpace(text.templateId));
    }

    [Serializable]
    public sealed class GameStateData
    {
        public int gold;
        public int level;
        public List<int> upgradeCounts = new();
    }

    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameLecture/Framework/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [field: SerializeField] public int InitialGold { get; private set; } = 1000;
        [field: SerializeField] public int InitialLevel { get; private set; } = 1;
    }

    public sealed class GameDataManager : Singleton<GameDataManager>
    {
        public Dictionary<int, ItemData> Items { get; private set; } = new();
        public Dictionary<string, LocalizedTextData> Texts { get; private set; } = new();
        public GameConfig Config { get; private set; }

        public void Load()
        {
            Config = DataManager.Instance.LoadConfig<GameConfig>("GameConfig");
            Items = DataManager.Instance.LoadJson<ItemDataLoader>("ItemData").MakeDictionary();
            Texts = DataManager.Instance.LoadJson<LocalizedTextDataLoader>("TextData").MakeDictionary();
        }
    }

    public sealed class GameManager : Singleton<GameManager>
    {
        public GameStateData State { get; set; } = new();

        public int Gold
        {
            get => State.gold;
            set
            {
                State.gold = value;
                EventManager.Instance.Trigger(GameEvent.GoldChanged);
            }
        }
    }

    public sealed class GameSaveManager : Singleton<GameSaveManager>
    {
        private readonly JsonSaveStore<GameStateData> _store = new("GameData.json");

        public void Save() => _store.Save(GameManager.Instance.State);

        public void Load()
        {
            var loaded = _store.Load();
            GameManager.Instance.State = loaded ?? CreateDefault();
        }

        public bool Delete() => _store.Delete();

        private static GameStateData CreateDefault()
        {
            var config = GameDataManager.Instance.Config;
            return new GameStateData
            {
                gold = config == null ? 0 : config.InitialGold,
                level = config == null ? 1 : config.InitialLevel
            };
        }
    }
}
