using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;
using System.Linq;

public class Path_AStar {

	Queue<Tile> path;

	//public constructor
	//generate the path from point A to B
	public Path_AStar(World world, Tile tileStart, Tile tileEnd){


		//chec to see if we havbe a valide tile graph
		if(world.tileGraph == null){
			world.tileGraph = new Path_TileGraph (world);
		}

		//A dictionary of all valid, walkable nodes
		Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

		Path_Node<Tile> start = nodes [tileStart];
		Path_Node<Tile> goal = nodes [tileEnd];

		//Make sur our start/end tiles are int the list of nodes !
		if (nodes.ContainsKey (tileStart) == false) {
			Debug.LogError ("path_AStar : the starting tile isn't in the list of nodes !");
			return;
		}

		if (nodes.ContainsKey (tileEnd) == false) {
			Debug.LogError ("path_AStar : the ending tile isn't in the list of nodes !");
			return;
		}

		//mostly following this pseudocode of wikipedia A*
		List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();

/*		List<Path_Node<Tile>> OpenSet = new List<Path_Node<Tile>>();
		OpenSet.Add( start );
*/
		//OpenSet c'est plus une List, mais une PriorityQueue, qui a une valeur qui détermine quel élément de cette "liste" est prioritaire
		SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>> ();
		OpenSet.Enqueue( start, 0);

		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>> ();

		Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (Path_Node<Tile> n in nodes.Values) {
			g_score [n] = Mathf.Infinity;
		}
		g_score[ start ] = 0;

		Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (Path_Node<Tile> n in nodes.Values) {
			f_score [n] = Mathf.Infinity;
		}
		f_score[ start ] = heuristic_cost_estimate( start, goal );

		while( OpenSet.Count > 0) {
			Path_Node<Tile> current = OpenSet.Dequeue();

			if (current == goal) {
				//we have reached our goal, let's convert this into an actual sequence of
				//tile to walk on, then end this constructor function !
				reconstruct_Path(Came_From, current);
				return;
			}

			ClosedSet.Add (current);
			foreach (Path_Edge<Tile> edge_neighbor in current.edges) {
				Path_Node<Tile> neighbor = edge_neighbor.node;
	
				if (ClosedSet.Contains(neighbor) == true) 
					continue; // ignore this already competed neighbor

				float tentative_g_score = g_score [current] + dist_between (current, neighbor);

				if (OpenSet.Contains (neighbor) && tentative_g_score >= g_score [neighbor])
					continue;
				
				Came_From[neighbor] = current;
				g_score [neighbor] = tentative_g_score;
				f_score [neighbor] = g_score [neighbor] + heuristic_cost_estimate (neighbor, goal);
				if (OpenSet.Contains (neighbor) == false) {
					OpenSet.Enqueue (neighbor, f_score [neighbor]);
				}
			
			} //foreach neighbour
		}//while
		//si on arrive ici, c'est qu'on a niqué le OpenSet sans avoir atteint un point current == goal
		//ça arrive quand y'a pas de chemin de start to goal, genre un PUTAIN DE MUR qui nique tout
		//sooo, pathlist will be null
	}

	float heuristic_cost_estimate( Path_Node<Tile> a, Path_Node<Tile> b){
		//squareroot
		return Mathf.Sqrt (
			Mathf.Pow (a.data.X - b.data.X, 2) +
			Mathf.Pow (a.data.Y - b.data.Y, 2)
		);
	}

	float dist_between( Path_Node<Tile> a, Path_Node<Tile> b)  {
		//we cam ma ke assumtion because we know we're working on the grin a this point
		//Hori/vert neigbours have a distance of 1
		if (Mathf.Abs (a.data.X - b.data.X) + Mathf.Abs (a.data.Y - b.data.Y) == 1) {
			return 1f;
		}
		//diag neighbours have a distance of 1,414.....
		if (Mathf.Abs (a.data.X - b.data.X) == 1 && Mathf.Abs (a.data.Y - b.data.Y) == 1) {
			return 1.41421356237f;
		}
		//otherwise do the actual math
		return Mathf.Sqrt (
			Mathf.Pow (a.data.X - b.data.X, 2) +
			Mathf.Pow (a.data.Y - b.data.Y, 2)
		);

	}

	void reconstruct_Path(
		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From,
		Path_Node<Tile> current
	) {
		//at this point, current IS the goal
		//so what whe want to do is walk backwards through the Came_From map
		//until we reach the "end" of that map... which will be our starting node

		Queue<Tile> total_path = new Queue<Tile>();
		total_path.Enqueue (current.data);// the "final" step in the path is the goal

		while (Came_From.ContainsKey (current)) {
			//came from is a map, where the key => value relation is real 
			//saying some node => we_got_there_from_thius_node
			current = Came_From [current];
			total_path.Enqueue (current.data);
		}

		// at this point, total_path is a queue that is running backwards from the EN tile to the Start tile, 
		// so let's reverse it

		path = new Queue<Tile>(total_path.Reverse() );

	}

	public Tile GetNextTile(){
		return path.Dequeue ();
	}
}
