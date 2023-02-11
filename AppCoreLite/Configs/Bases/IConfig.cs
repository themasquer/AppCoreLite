using AppCoreLite.Enums;

namespace AppCoreLite.Configs.Bases
{
    /// <summary>
    /// Language configuration interface used in config, model, service and manager classes.
    /// </summary>
    public interface IConfig
    {
        void Set(Languages language);
    }
}
