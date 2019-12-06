

public class ScanError
{
    public int Line { get; private set; }

    public string Message { get; private set; }

    public ScanError(int line, string message)
    {
        Line = line;
        Message = message;
    }
}