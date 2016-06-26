using UnityEngine;
using System.Collections;

public class Path_Node<T> {

	//on intitlise T parce que c'est une convention
	public T data;

	public Path_Edge<T>[] edges; //nodes leading OUT from this node

}
