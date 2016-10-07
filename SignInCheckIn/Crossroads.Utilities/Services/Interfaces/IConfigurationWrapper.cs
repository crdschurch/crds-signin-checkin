namespace Crossroads.Utilities.Interfaces
{
    public interface IConfigurationWrapper
    {
        int GetConfigIntValue(string key);
        string GetConfigValue(string key);
        string GetEnvironmentVarAsString(string variable);
        string GetEnvironmentVarAsString(string variable, bool throwIfNotFound);
    }
}
