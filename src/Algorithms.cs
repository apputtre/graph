using CSGraph.ExtensionMethods;

namespace CSGraph
{
    public static class Algorithms
    {

        public static void Test()
        {
            PriorityQueue<int, int> queue = new();
            queue.SetPriority(5, 5);
        }

        #nullable enable
        public static void Dijkstra<V>
        (
            WeightedGraph<int, V> g,
            V source,
            out Dictionary<V, int> costs,
            out Dictionary<V, V?> routes
        )
            where V : struct
        {
            costs = new();
            routes = new();

            // create a queue of nodes to visit
            MinPriorityQueue<V> to_visit = new();

            // initialize costs, routes, and to_visit

            foreach (V v in g.Vertices)
            {
                if (v.Equals(source))
                    costs[v] = 0;
                else
                    costs[v] = int.MaxValue;

                routes[v] = null;

                to_visit.Insert(v, costs[v]);
            }

            while (to_visit.Count > 0)
            {
                // find the node in to_visit which has the least cost path from the start node
                V next = to_visit.Extract();

                foreach (V neighbor in g.GetNeighbors(next))
                {
                    int neighborCost = g.GetEdgeData(next, neighbor);

                    // if the path to neighbor through this node is less than that of the existing path
                    int cost = costs[next] + neighborCost;
                    if (cost < costs[neighbor])
                    {
                        // update the costs and routes tables
                        to_visit.Update(neighbor, cost);
                        costs[neighbor] = cost;
                        routes[neighbor] = next;
                    }
                }
            }

            return;
        }

        // uses Dijkstra's algorithm to find the shortest path from "from" to "to" on graph "graph"
        #nullable enable
        public static List<V> ShortestPath<V>(WeightedGraph<int, V> g, V from, V to)
            where V : struct
        {
            Dictionary<V, int> costs = new();
            Dictionary<V, V?> routes = new();

            Dijkstra(g, from, out costs, out routes);

            return ShortestPath(routes, to);
        }

        // finds the shortest path given the costs table generated by Dijkstra's algorithm
        public static List<V> ShortestPath<V>(Dictionary<V, V?> routes, V to)
            where V : struct
        {
            List<V> ret = new() {to};

            V v = to;
            while(routes[v] != null)
            {
                V next = routes[v].GetValueOrDefault();
                ret.Add(next);
                v = next;
            }

            ret.Reverse();

            return ret;
        }

        public static WeightedGraph<int, V> Prims<V>(WeightedGraph<int, V> g, V root)
            where V : struct
        {
            WeightedGraph<int, V> minSpanTree = new();

            Dictionary<V, int> costs = new();
            Dictionary<V, V?> parents = new();
            MinPriorityQueue<V> queue = new();

            foreach (V v in g.Vertices)
            {
                minSpanTree.AddVertex(v);
                queue.Insert(v, int.MaxValue);
                costs[v] = int.MaxValue;
                parents[v] = null;
            }

            queue.Update(root, 0);
            costs[root] = 0;

            while(queue.Count > 0)
            {
                V u = queue.Extract(); 
                
                if (parents[u] != null)
                    minSpanTree.AddEdge(u, parents[u].GetValueOrDefault(), costs[u]);
                
                V[] neighbors = g.GetNeighbors(u);

                foreach (V neighbor in neighbors)
                {
                    int cost = g.GetEdgeData(u, neighbor);

                    if (queue.Elements.Contains(neighbor) && cost < costs[neighbor])
                    {
                        queue.Update(neighbor, cost);
                        parents[neighbor] = u;
                        costs[neighbor] = cost;
                    }
                }
            }

            return minSpanTree;
        }

        public static WeightedGraph<int, V> MST<V>(WeightedGraph<int, V> g, V root)
            where V : struct
        {
            return Prims(g, root);
        }
    }
}