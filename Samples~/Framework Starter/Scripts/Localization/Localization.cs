using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameLecture.Framework.Samples
{
    [Serializable]
    public sealed class LanguageFont
    {
        public GameLanguage language;
        public TMP_FontAsset font;
    }

    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "GameLecture/Framework/Localization Config")]
    public sealed class LocalizationConfig : ScriptableObject
    {
        [SerializeField] private List<LanguageFont> fonts = new();

        public TMP_FontAsset GetFont(GameLanguage language) =>
            fonts.Find(entry => entry.language == language)?.font;
    }

    public sealed class LocalizationManager : Singleton<LocalizationManager>
    {
        [SerializeField] private LocalizationConfig config;
        private TMP_FontAsset _font;
        private GameLanguage _language = GameLanguage.Korean;

        public GameLanguage Language
        {
            get => _language;
            set
            {
                if (_language == value)
                    return;
                _language = value;
                _font = null;
                EventManager.Instance.Trigger(GameEvent.LanguageChanged);
            }
        }

        public TMP_FontAsset CurrentFont => _font ??= config == null ? null : config.GetFont(Language);

        public string GetText(string templateId)
        {
            if (!GameDataManager.Instance.Texts.TryGetValue(templateId, out var text))
                return string.Empty;
            return Language == GameLanguage.Korean ? text.korean : text.english;
        }
    }

    public static class LocalizationExtensions
    {
        public static void SetLocalizedText(this TMP_Text text, string templateId)
        {
            if (text == null)
                return;
            text.text = LocalizationManager.Instance.GetText(templateId);
            var font = LocalizationManager.Instance.CurrentFont;
            if (font != null)
                text.font = font;
        }
    }

    public abstract class LocalizedUIBase : UIBase
    {
        protected virtual void OnEnable() =>
            EventManager.Instance.Add(GameEvent.LanguageChanged, RefreshUI);

        protected virtual void OnDisable() =>
            EventManager.Instance.Remove(GameEvent.LanguageChanged, RefreshUI);
    }
}
