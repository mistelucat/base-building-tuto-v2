using UnityEngine;
using System.Collections;

public class Path_Edge<T> {

	//encore une data class generic, parce que <T>

	public float cost; // Cost to traverse this edge (i.e cost to ENTER the tile)
	public Path_Node<T> node;

}
