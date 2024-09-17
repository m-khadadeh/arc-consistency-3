using ArcConsistency3;

namespace ArcConsistency3.Tests
{
  public class NumberUnitTests
  {
    List<int> domain = new List<int>() {1,2,3,4,5,6,7};

    bool ConstrainerNotEqual(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      if(sourceList.Count == 1) {
      targetList.Remove(sourceList.First());
      return targetList.Count != initialtargetListCount;
      }
      return false;
    }

    bool ConstrainerEqual(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      targetList.RemoveAll(e => !sourceList.Contains(e));
      return targetList.Count != initialtargetListCount;
    }

    bool ConstrainerDifferentParity(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      int parity = -1;
      int parityCount = 0;
      foreach(var number in sourceList) {
      if(number % 2 != parity) {
        parity = number % 2;
        parityCount++;
        if(parityCount == 2) {
        return false;
        }
      }
      }
      targetList.RemoveAll((e) => e % 2 == parity);
      return targetList.Count != initialtargetListCount;
    }

    bool ConstrainerSameParity(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      int parity = -1;
      int parityCount = 0;
      foreach(var number in sourceList) {
      if(number % 2 != parity) {
        parity = number % 2;
        parityCount++;
        if(parityCount == 2) {
        return false;
        }
      }
      }
      targetList.RemoveAll((e) => e % 2 != parity);
      return targetList.Count != initialtargetListCount;
    }

    bool ConstrainerGreaterThan(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      int max = int.MinValue;
      foreach(var number in sourceList) {
      if(number > max) {
        max = number;
      }
      }
      targetList.RemoveAll((e) => e >= max);
      return targetList.Count != initialtargetListCount;
    }

    bool ConstrainerLessThan(List<int> sourceList, List<int> targetList) {
      int initialtargetListCount = targetList.Count;
      int min = int.MaxValue;
      foreach(var number in sourceList) {
      if(number < min) {
        min = number;
      }
      }
      targetList.RemoveAll((e) => e <= min);
      return targetList.Count != initialtargetListCount;
    }

    ArcConsistency3Graph<int> graph;

    [Fact]
    public void TestGraphCreation() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList(), domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
          new Arc<int>(0, 2, ConstrainerSameParity),
          new Arc<int>(2, 0, ConstrainerSameParity),
          new Arc<int>(0, 3, ConstrainerLessThan),
          new Arc<int>(3, 0, ConstrainerGreaterThan),
          new Arc<int>(1, 2, ConstrainerNotEqual),
          new Arc<int>(2, 1, ConstrainerNotEqual),
          new Arc<int>(2, 3, ConstrainerDifferentParity),
          new Arc<int>(3, 2, ConstrainerDifferentParity),
        }
      );

      List<int> node0List = graph.Nodes[0].ToList();
      List<int> node1List = graph.Nodes[1].ToList();
      List<int> node2List = graph.Nodes[2].ToList();
      List<int> node3List = graph.Nodes[3].ToList();

      node0List.Sort();
      node1List.Sort();
      node2List.Sort();
      node3List.Sort();

      Assert.Equal(new List<int>{1,2,3,4,5,6}, node0List);
      Assert.Equal(new List<int>{1,2,3,4,5,6}, node1List);
      Assert.Equal(new List<int>{1,2,3,4,5,6,7}, node2List);
      Assert.Equal(new List<int>{2,3,4,5,6,7}, node3List);
    }

    [Fact]
    public void TestSetValue() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList(), domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
          new Arc<int>(0, 2, ConstrainerSameParity),
          new Arc<int>(2, 0, ConstrainerSameParity),
          new Arc<int>(0, 3, ConstrainerLessThan),
          new Arc<int>(3, 0, ConstrainerGreaterThan),
          new Arc<int>(2, 3, ConstrainerDifferentParity),
          new Arc<int>(3, 2, ConstrainerDifferentParity),
        }
      );

      graph.SetNode(1, 4);
      
      List<int> node0List = graph.Nodes[0].ToList();
      List<int> node1List = graph.Nodes[1].ToList();
      List<int> node2List = graph.Nodes[2].ToList();
      List<int> node3List = graph.Nodes[3].ToList();

      node0List.Sort();
      node1List.Sort();
      node2List.Sort();
      node3List.Sort();

      Assert.Equal(new List<int>{4}, node0List);
      Assert.Equal(new List<int>{4}, node1List);
      Assert.Equal(new List<int>{2,4,6}, node2List);
      Assert.Equal(new List<int>{5,7}, node3List);

      Assert.False(graph.IsNodeSet(0));
      Assert.True(graph.IsNodeSet(1));
      Assert.False(graph.IsNodeSet(2));
      Assert.False(graph.IsNodeSet(3));
    }

    [Fact]
    public void TestUnsetValue() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList(), domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
          new Arc<int>(0, 2, ConstrainerSameParity),
          new Arc<int>(2, 0, ConstrainerSameParity),
          new Arc<int>(0, 3, ConstrainerLessThan),
          new Arc<int>(3, 0, ConstrainerGreaterThan),
          new Arc<int>(2, 3, ConstrainerDifferentParity),
          new Arc<int>(3, 2, ConstrainerDifferentParity),
        }
      );

      graph.SetNode(1, 4);
      graph.SetNode(2, 2);
      graph.UnsetNode(1);
      
      List<int> node0List = graph.Nodes[0].ToList();
      List<int> node1List = graph.Nodes[1].ToList();
      List<int> node2List = graph.Nodes[2].ToList();
      List<int> node3List = graph.Nodes[3].ToList();

      node0List.Sort();
      node1List.Sort();
      node2List.Sort();
      node3List.Sort();

      Assert.Equal(new List<int>{2,4,6}, node0List);
      Assert.Equal(new List<int>{2,4,6}, node1List);
      Assert.Equal(new List<int>{2}, node2List);
      Assert.Equal(new List<int>{3,5,7}, node3List);

      Assert.False(graph.IsNodeSet(0));
      Assert.False(graph.IsNodeSet(1));
      Assert.True(graph.IsNodeSet(2));
      Assert.False(graph.IsNodeSet(3));
    }

    [Fact]
    public void TestContradiction() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerNotEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
        }
      );

      Assert.False(graph.SetNode(1, 4));
    }
    
    [Fact]
    public void TestContradictionOnCreation() {
      Assert.Throws<InvalidOperationException>(() => {
          graph = new ArcConsistency3Graph<int>(
          new List<int>[] {domain.ToList(), domain.ToList()},
          new List<Arc<int>>() {
            new Arc<int>(0, 1, ConstrainerLessThan),
            new Arc<int>(1, 0, ConstrainerLessThan),
          }
        );
      });
    }

    [Fact]
    public void TestSetSameNode() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
        }
      );

      graph.SetNode(1, 4);

      Assert.Throws<InvalidOperationException>(() => {
        graph.SetNode(1, 3);
      });
    }

    [Fact]
    public void TestSetIncorrectValue() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
        }
      );

      graph.SetNode(1, 4);

      Assert.Throws<InvalidOperationException>(() => {
        graph.SetNode(0, 3);
      });
    }

    [Fact]
    public void TestUnsetNonsetNode() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
        }
      );

      Assert.Throws<InvalidOperationException>(() => {
        graph.UnsetNode(0);
      });
    }

    [Fact]
    public void TestIsGraphAllSet() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList(), domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
          new Arc<int>(0, 2, ConstrainerSameParity),
          new Arc<int>(2, 0, ConstrainerSameParity),
          new Arc<int>(0, 3, ConstrainerLessThan),
          new Arc<int>(3, 0, ConstrainerGreaterThan),
          new Arc<int>(2, 3, ConstrainerDifferentParity),
          new Arc<int>(3, 2, ConstrainerDifferentParity),
        }
      );

      graph.SetNode(0, 4);
      graph.SetNode(1, 4);
      graph.SetNode(2, 2);
      Assert.False(graph.IsAllSet());

      graph.SetNode(3, 5);
      Assert.True(graph.IsAllSet());

      graph.UnsetNode(0);
      Assert.False(graph.IsAllSet());
    }

    [Fact]
    public void TestReset() {
      graph = new ArcConsistency3Graph<int>(
      new List<int>[] {domain.ToList(), domain.ToList(), domain.ToList(), domain.ToList()},
        new List<Arc<int>>() {
          new Arc<int>(0, 1, ConstrainerEqual),
          new Arc<int>(1, 0, ConstrainerEqual),
          new Arc<int>(0, 2, ConstrainerSameParity),
          new Arc<int>(2, 0, ConstrainerSameParity),
          new Arc<int>(0, 3, ConstrainerLessThan),
          new Arc<int>(3, 0, ConstrainerGreaterThan),
          new Arc<int>(2, 3, ConstrainerDifferentParity),
          new Arc<int>(3, 2, ConstrainerDifferentParity),
        }
      );

      graph.SetNode(0, 4);
      graph.SetNode(1, 4);
      graph.SetNode(2, 2);
      graph.SetNode(3, 5);

      graph.ResetToInitialConditions();

      List<int> node0List = graph.Nodes[0].ToList();
      List<int> node1List = graph.Nodes[1].ToList();
      List<int> node2List = graph.Nodes[2].ToList();
      List<int> node3List = graph.Nodes[3].ToList();

      node0List.Sort();
      node1List.Sort();
      node2List.Sort();
      node3List.Sort();

      Assert.Equal(new List<int>{1,2,3,4,5,6}, node0List);
      Assert.Equal(new List<int>{1,2,3,4,5,6}, node1List);
      Assert.Equal(new List<int>{1,2,3,4,5,6,7}, node2List);
      Assert.Equal(new List<int>{2,3,4,5,6,7}, node3List);

      Assert.False(graph.IsNodeSet(0));
      Assert.False(graph.IsNodeSet(1));
      Assert.False(graph.IsNodeSet(2));
      Assert.False(graph.IsNodeSet(3));
    }
  }
}