using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Room : MonoBehaviour
{
	/// <summary>
	/// Stores rooms connected to the room
	/// </summary>
	public class AdjacentRooms
	{
		public class RoomConnection
		{
			public RoomConnection(){}
			public RoomConnection(Room origin, Room destination)//constructor
			{
				this.origin = origin;//sets origin = to variable passed in as origin
				this.destination = destination;//sets destination = to variable passed in as destination
			}
			Room origin;//Room that is at the center
			Room destination;//Room that is adjacent to the origin room
		}
		/// <summary>
		/// Connection to room above origin
		/// </summary>
		private RoomConnection connectionAbove;
		/// <summary>
		/// Connection to room below origin
		/// </summary>
		private RoomConnection connectionBelow;
		/// <summary>
		/// Connection to room left of origin
		/// </summary>
		private RoomConnection connectionLeft;
		/// <summary>
		/// Connection to room right of origin
		/// </summary>
		private RoomConnection connectionRight;
		/// <summary>
		/// Room located above the current room
		/// </summary>
		private Room roomAbove;
		/// <summary>
		/// Room located below the current room
		/// </summary>
		private Room roomBelow;
		/// <summary>
		/// Room located left of the current room
		/// </summary>
		private Room roomLeft;
		/// <summary>
		/// Room located right of the current room
		/// </summary>
		private Room roomRight;
		/// <summary>
		/// Stores the room as above the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomAbove(Room room) { roomAbove = room; }
		/// <summary>
		/// Stores the room as below the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomBelow(Room room) { roomBelow = room; }
		/// <summary>
		/// Stores the room as left of the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomLeft(Room room) { roomLeft = room; }
		/// <summary>
		/// Stores the room as right of the current room
		/// </summary>
		/// <param name="room"></param>
		public void SetRoomRight(Room room) { roomRight = room; }
		public Room GetRoomAbove(){return roomAbove;}
		public Room GetRoomBelow(){return roomBelow;}
		public Room GetRoomLeft(){return roomLeft;}
		public Room GetRoomRight(){return roomRight;}
		public void SetConnectionAbove(Room origin, Room destination)
		{ connectionAbove = new RoomConnection(origin, destination); }
		public void SetConnectionBelow(Room origin, Room destination)
		{ connectionBelow = new RoomConnection(origin, destination); }
		public void SetConnectionLeft(Room origin, Room destination)
		{connectionLeft = new RoomConnection(origin, destination); }
		public void SetConnectionRight(Room origin, Room destination)
		{connectionRight = new RoomConnection(origin, destination); }
		public RoomConnection GetConnectionAbove(){return connectionAbove;}
		public RoomConnection GetConnectionBelow(){return connectionBelow;}
		public RoomConnection GetConnectionLeft(){return connectionLeft;}
		public RoomConnection GetConnectionRight(){return connectionRight;}
	}

	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the top
	/// </summary>
	[SerializeField]
	private bool hasTopConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the bottom
	/// </summary>
	[SerializeField]
	private bool hasBottomConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the left
	/// </summary>
	[SerializeField]
	private bool hasLeftConnection;
	/// <summary>
	/// BOOLEAN: Determines if the room has a connection towards the right
	/// </summary>
	[SerializeField]
	private bool hasRightConnection;
	/// <summary>
	/// BOOL: Determines if the top connection of the room is satisfied
	/// </summary>
	private bool topIsConnected;
	[SerializeField]
	/// <summary>
	/// BOOL: Determines if the bottom connection of the room is satisfied
	/// </summary>
	private bool bottomIsConnected;
	/// <summary>
	/// BOOL: Determines if the left connection of the room is satisfied
	/// </summary>
	private bool leftIsConnected;
	/// <summary>
	/// BOOL: Determines if the right connection of the room is satisfied
	/// </summary>
	private bool rightIsConnected;
	/// <summary>
	/// BOOLEAN: Determines if all the connections in the room have been satisfied
	/// </summary>
	private bool allConnected;
	/// <summary>
	/// BOOLEAN: Determines if the room is the origin
	/// </summary>
	private bool isOrigin;
	/// <summary>
	/// BOOLEAN: Determines if the room has stairs leading upwards
	/// </summary>
	private bool hasStairsUp;
	/// <summary>
	/// BOOLEAN: Determines if the room has stairs leading downwards
	/// </summary>
	private bool hasStairsDown;
	/// <summary>
	/// INT: Determines what floor the room is on;
	/// </summary>
	private int floorLevel;
	private Vector3 roomPosition;
	public AdjacentRooms adjacentRooms;






	/// <summary>
	/// Returns true if the room has a top connection
	/// </summary>
	/// <returns>hasTopConnection</returns>
	public bool HasTopCon() { return hasTopConnection; }
	/// <summary>
	/// Returns true if the room has a bottom connection
	/// </summary>
	/// <returns>hasBottomConnection</returns>
	public bool HasBottomCon() { return hasBottomConnection; }
	/// <summary>
	/// Returns true if the room has a left connection
	/// </summary>
	/// <returns>hasLeftConnection</returns>
	public bool HasLeftCon() { return hasLeftConnection; }
	/// <summary>
	/// Returns true if the room has a right connection
	/// </summary>
	/// <returns>hasRightConnection</returns>
	public bool HasRightCon() { return hasRightConnection; }
	/// <summary>
	/// Returns true if the top connection is satisfied
	/// </summary>
	/// <returns>topIsConnected</returns>
	public bool TopIsCon() { return topIsConnected; }
	/// <summary>
	/// Returns true if the bottom connection is satisfied
	/// </summary>
	/// <returns>bottomIsConnected</returns>
	public bool BottomIsCon() { return bottomIsConnected; }
	/// <summary>
	/// Returns true if the left connection is satisfied
	/// </summary>
	/// <returns>leftIsConnected</returns>
	public bool LeftIsCon() { return leftIsConnected; }
	/// <summary>
	/// Returns true if the right connection is satisfied
	/// </summary>
	/// <returns>rightIsConnected</returns>
	public bool RightIsCon() { return rightIsConnected; }
	/// <summary>
	/// Sets top connected boolean to true
	/// </summary>
	public void ConTop() { topIsConnected = true; }
	/// <summary>
	/// Sets bottom connected boolean to true
	/// </summary>
	public void ConBottom() { bottomIsConnected = true; }
	/// <summary>
	/// Sets left connected boolean to true
	/// </summary>
	public void ConLeft() { leftIsConnected = true; }
	/// <summary>
	/// Sets right connected boolean to true
	/// </summary>
	public void ConRight() { rightIsConnected = true; }
	/// <summary>
	/// Returns true if all the connections in the room are satisfied
	/// </summary>
	/// <returns>allConnected</returns>
	public bool AllConnected() { return allConnected; }
	public void SetAllConnected() { allConnected = true; }
	/// <summary>
	/// Returns true if the room is the origin
	/// </summary>
	/// <returns>hasRightConnection</returns>
	public bool IsOrigin() { return isOrigin; }
	public void MakeOrigin() { isOrigin = true; }
	/// <summary>
	/// Returns true if the room has stairs leading upwards
	/// </summary>
	/// <returns>hasStairsUp</returns>
	public bool HasStairsup() { return hasStairsUp; }
	/// <summary>
	/// Returns true if the room has stairs leading downwards
	/// </summary>
	/// <returns>hasStairsDown</returns>
	public bool HasStairsdown() { return hasStairsDown; }
	/// <summary>
	/// Returns the floor level the room is on
	/// </summary>
	/// <returns>floorLevel</returns>
	public int GetFloorLevel() { return floorLevel; }
	public void SetRoomPosition(Vector3 position) { roomPosition = position; }
	/// <summary>
	/// Returns the position of the room
	/// </summary>
	/// <returns>roomPosition</returns>
	public Vector3 GetRoomPosition() { return roomPosition; }


}
