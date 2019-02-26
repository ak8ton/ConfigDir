namespace ConfigDir.Data
{
    /// <summary>
    /// Config event handler
    /// </summary>
    /// <param name="args"></param>
    public delegate void ConfigEventHandler(ConfigEventArgs args);

    /// <summary>
    /// Config error event handler
    /// </summary>
    /// <param name="args"></param>
    public delegate void ConfigErrorEventHandler(ConfigErrorEventArgs args);
}
