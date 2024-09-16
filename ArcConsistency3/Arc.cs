namespace ArcConsistency3
{
  public class Arc<T> {
    // Constrainer function returns true if there was a change made to the target set
    public delegate bool Constrainer(HashSet<T> sourceSet, HashSet<T> targetSet);
    
    public int SourceIndex {get; private set;}
    public int TargetIndex {get; private set;}
    public Constrainer ApplyConstrainer {get; private set;}

    public Arc(int sourceIndex, int targetIndex, Constrainer constrainer) {
      SourceIndex = sourceIndex;
      TargetIndex = targetIndex;
      ApplyConstrainer = constrainer;  
    }
  }
}