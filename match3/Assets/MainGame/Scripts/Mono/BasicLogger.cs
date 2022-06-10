using UnityEngine;
public class BasicLogger : Singleton<BasicLogger> {

    public TextMesh text;
    public static void Log(string text) {
        instance.text.text = text;
    }

}