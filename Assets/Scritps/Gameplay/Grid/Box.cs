public enum BoxType
{
    QuestionBox,
    ExplosiveBox
}

[System.Serializable]
public struct Box
{
    public BoxType Type;
    public int XPosition;
    public int YPosition;
}
