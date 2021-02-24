public class Disposable
{
    private string Uid;

    public Disposable()
    {
        Uid = SysUtil.GetUid();
    }

    public string GetUid()
    {
        return Uid;
    }
}
