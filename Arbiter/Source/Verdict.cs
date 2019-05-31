namespace Arbiter
{
    public enum Verdict
    {
        None,
        Accept,
        Wrong,
        TimeLimit,
        MemoryLimit,
        IdleLimit,
        CompilationError,
        RuntimeError,
        PresentationError
    }
}