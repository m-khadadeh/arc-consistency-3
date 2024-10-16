using System;
using System.Collections.Generic;

namespace ArcConsistency3
{
  public class ArcConsistency3Graph<T>
  {    
    // Nodes are just a set of their possible values.
    // We'll store them in an array because the number of nodes won't change
    private List<T>[] _nodes;
    private List<T>[] _initialConditions;

    // We'll store a bool for whether a node has been set or not.
    private bool[] _setNodes;

    // The index of this array will be the index of the source node of the arcs
    private List<Arc<T>>[] _edgeLists;

    // We'll use a Hashset for the worklist so we dont add duplicates during propagation
    private Queue<Arc<T>> _workList;

    public IEnumerable<T>[] Nodes {
      get { return _nodes; }
    }

    private bool Propagate() {
      while (_workList.Count > 0) {
        Arc<T> arc = _workList.Dequeue();

        if(arc.ApplyConstrainer(_nodes[arc.SourceIndex], _nodes[arc.TargetIndex])) {
          // A change was made upon application of the rule.
          if(_nodes[arc.TargetIndex].Count == 0) {
            _workList.Clear();
            return false; // Contradiction
          }
          
          // Queue up arcs coming from target
          foreach(var outGoingArc in _edgeLists[arc.TargetIndex]) {
            if(outGoingArc.SourceIndex != arc.SourceIndex) {
              _workList.Enqueue(outGoingArc);
            }
          }
        }
      }
      return true; // No contradictions
    }

    public bool SetNode(int nodeIndex, T nodeValue) {
      // Returns false if there is a contradiction, otherwise true.
      
      if(_setNodes[nodeIndex])
      {
        throw new InvalidOperationException($"Node {nodeIndex} already set.");
      }
      if(!_nodes[nodeIndex].Contains(nodeValue)) {
        throw new InvalidOperationException($"{nodeValue} is not a possible choice for Node {nodeIndex}");
      }
      _setNodes[nodeIndex] = true;
      _nodes[nodeIndex].Clear();
      _nodes[nodeIndex].Add(nodeValue);

      foreach(var arc in _edgeLists[nodeIndex]) {
        _workList.Enqueue(arc);
      }

      return Propagate();
    }

    public bool UnsetNode(int nodeIndex) {
      if(!_setNodes[nodeIndex]) {
        throw new InvalidOperationException($"Node {nodeIndex} not set.");
      }
      _setNodes[nodeIndex] = false;

      for(int i = 0; i < _nodes.Length; i++) {
        if(!_setNodes[i]) {
          _nodes[i] = new List<T>(_initialConditions[i]);
        }
        foreach(var arc in _edgeLists[i]) {
          if(!_setNodes[arc.TargetIndex]) {
            _workList.Enqueue(arc);
          }
        }
      }

      return Propagate();
    }
    
    public ArcConsistency3Graph(List<T>[] nodes, List<Arc<T>> arcs) {
      _initialConditions = nodes;
      
      // Convert arc list into edge list
      _edgeLists = new List<Arc<T>>[_initialConditions.Length];
      for(int i = 0; i < _initialConditions.Length; i++) {
        _edgeLists[i] = new List<Arc<T>>();
      }
      foreach(var arc in arcs) {
        _edgeLists[arc.SourceIndex].Add(arc);
      }

      _nodes = new List<T>[_initialConditions.Length];
      _setNodes = new bool[_nodes.Length];
      _workList = new Queue<Arc<T>>();
      ResetToInitialConditions();
    }

    public void ResetToInitialConditions() {
      for (int i = 0; i < _nodes.Length; i++) {
        _nodes[i] = new List<T>(_initialConditions[i]);
        _setNodes[i] = false;
        foreach(var edge in _edgeLists[i]) {
          _workList.Enqueue(edge);
        }
      }
      if(!Propagate()) {
        throw new InvalidOperationException("Initial arc conditions result in a contradiction.");
      }
    }

    public bool IsAllSet() {
      foreach(var setValue in _setNodes) {
        if(!setValue) return false;
      }
      return true;
    }

    public bool IsNodeSet(int nodeIndex) {
      return _setNodes[nodeIndex];
    }
  }
}
