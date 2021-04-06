using System.Collections.Generic;

[System.Serializable]
public class SoundName{
    public string soundName;
}
[System.Serializable]
public class SoundString {
    public List<SoundName> SoundNames = new List<SoundName>();
}