using System;
using SingleApp;

public class AGameManager : PersistentSingleton<AGameManager>
{
    private TeleDataManager _teleData;
    
    public static Action<int> OnChangeCoin;
    public static Action<int> OnChangeDiamond;
    public static Action<bool> OnPlayMusic;
    public static Action<int> OnChooseSong;
    public static Action<float> SetTimeStop;
    public static Action<float> SetRegisterTime;
}