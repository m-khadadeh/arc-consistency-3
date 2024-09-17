using ArcConsistency3;

namespace ArcConsistency3.Tests{
  public class QueensTests {
    int[] domain = {0, 1, 2, 3};

    bool QueensConstrainer(List<int> sourceList, List<int> targetList, int distance) {
      int initialtargetListCount = targetList.Count;

      Dictionary<int, int> countsForRemoval = targetList.ToDictionary(x => x, x => 0);

      foreach(var possibility in sourceList) {
        if(targetList.Contains(possibility)) {
          // Target is vertically on the same line.
          countsForRemoval[possibility]++;
        }
        if(targetList.Contains(possibility - distance)) {
          // Target is on the incoming diagonal line
          countsForRemoval[possibility - distance]++;
        }
        if(targetList.Contains(possibility + distance)) {
          // Target is on the outgoing diagonal line
          countsForRemoval[possibility + distance]++;
        }
      }

      foreach(var count in countsForRemoval) {
        if(count.Value == sourceList.Count) {
          // This element of the targetList has been removed by every element of the sourceList.
          // Remove it
          targetList.Remove(count.Key);
        }
      }

      return initialtargetListCount != targetList.Count;
    }

    [Fact]
    public void TestFourQueensWithBacktracking() {
      List<int>[] nodes = new List<int>[domain.Length];
      for(int i = 0; i < nodes.Length; i++) {
        nodes[i] = domain.ToList();
      }

      List<Arc<int>> arcs = new List<Arc<int>>();
      for(int startNode = 0; startNode < domain.Length; startNode++) {
        for(int distance = 1; distance < domain.Length; distance++) {
          int tileDistance = distance;
          if(startNode - distance >= 0){
            arcs.Add(new Arc<int>(startNode, startNode - distance, (List<int> sourceList, List<int> targetList) => QueensConstrainer(sourceList, targetList, tileDistance)));
          }
          if(startNode + distance < domain.Length){
            arcs.Add(new Arc<int>(startNode, startNode + distance, (List<int> sourceList, List<int> targetList) => QueensConstrainer(sourceList, targetList, tileDistance)));
          }
        }
      }

      ArcConsistency3Graph<int> queenPuzzleGraph = new ArcConsistency3Graph<int>(nodes, arcs);

      Stack<BacktrackingState> backtrackingStack = new Stack<BacktrackingState>();

      BacktrackingState state = new BacktrackingState(0, 0);

      while (!queenPuzzleGraph.IsAllSet()) {
        while(!queenPuzzleGraph.Nodes[state.currentRow].Contains(state.currentColumn)) {
          if(state.currentColumn >= domain.Length) {
            state = backtrackingStack.Pop();
            queenPuzzleGraph.UnsetNode(state.currentRow);
            state.currentColumn++;
          }
          else {
            state.currentColumn++;
          }
        }
        if(queenPuzzleGraph.SetNode(state.currentRow, state.currentColumn)) {
          backtrackingStack.Push(new BacktrackingState(state.currentRow, state.currentColumn));
          state.currentRow++;
          state.currentColumn = 0;
        }
        else {
          queenPuzzleGraph.UnsetNode(state.currentRow);
          state.currentColumn++;
        }
      }

      Assert.True(queenPuzzleGraph.IsAllSet());
      Assert.True(queenPuzzleGraph.Nodes[0].ElementAt(0) == 1);
      Assert.True(queenPuzzleGraph.Nodes[1].ElementAt(0) == 3);
      Assert.True(queenPuzzleGraph.Nodes[2].ElementAt(0) == 0);
      Assert.True(queenPuzzleGraph.Nodes[3].ElementAt(0) == 2);
    }

    struct BacktrackingState {
      public BacktrackingState(int row, int column) {
        currentRow = row;
        currentColumn = column;
      }
      public int currentRow;
      public int currentColumn;
    }
  }
}
