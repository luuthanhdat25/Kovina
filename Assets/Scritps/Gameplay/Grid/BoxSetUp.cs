public enum BoxType
{
    QuestionBox,
    ExplosiveBox
}

[System.Serializable]
public struct BoxSetUp
{
    public BoxType Type;
    public int XPosition;
    public int YPosition;
}
