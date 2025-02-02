﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QPath {

	public class QPath_AStar<T> where T : IQPathTile {
		// The thing that actually does pathfinding stuff

		Queue<T> path;
		IQPathUnit unit;
		T startTile;
		T endTile;
		private CostEstimateDelegate costEstimateFunc;

		public QPath_AStar( IQPathUnit unit, T startTile, T endTile, CostEstimateDelegate costEstimateFunc ) {
			// Do setup
			this.unit = unit;
			this.startTile = startTile;
			this.endTile = endTile;
			this.costEstimateFunc = costEstimateFunc;
		}

		public void DoWork() {

			path = new Queue<T>();

			HashSet<T> closedSet = new HashSet<T>();

			//A PriorityQueue is a type of array where each value has a priority value. Use Enqueue(value, priority) and Dequeue()
			PathfindingPriorityQueue<T> openSet = new PathfindingPriorityQueue<T>();
			openSet.Enqueue(startTile, 0);

			Dictionary<T, T> came_From = new Dictionary<T, T>();

			//The cost to walk from startTile A to tile B
            Dictionary<T, float> g_score = new Dictionary<T, float>();
            g_score[startTile] = 0;

            //The estimated cost to walk from tile A to endTile B
            Dictionary<T, float> f_score = new Dictionary<T, float>();
            f_score[startTile] = costEstimateFunc(startTile, endTile);
            
            while (openSet.Count > 0)
            {
	            T current = openSet.Dequeue();

                // Check to see if we are at destination.
                if ( System.Object.ReferenceEquals( current, endTile ) )
                {
                    Reconstruct_path(came_From, current);
                    return;
                }

                closedSet.Add(current);

                foreach (T neighbour in current.GetNeighbours())
                {
	                if (closedSet.Contains(neighbour))
                    {
                        continue; // ignore this already completed neighbor
                    }

                    float total_pathfinding_cost_to_neighbor = 
                        neighbour.AggregateCostToEnter( g_score[current], current, this.unit );

                    if(total_pathfinding_cost_to_neighbor < 0)
                    {
                        // Values less than zero represent an invalid/impassable tile
                        continue;
                    }
                    //Debug.Log(total_pathfinding_cost_to_neighbor);

                    float tentative_g_score = total_pathfinding_cost_to_neighbor;

                    if (openSet.Contains(neighbour) && tentative_g_score >= g_score[neighbour])
                    {
	                    // Is the neighbour already in the open set?
	                    // If so, and if this new score is worse than the old score, discard this new result.
                        continue;
                    }

                    // This is either a new tile or we just found a cheaper route to it
                    came_From[neighbour] = current;
                    g_score[neighbour] = tentative_g_score;
                    f_score[neighbour] = g_score[neighbour] + costEstimateFunc(neighbour, this.endTile);
                    //Debug.Log("EnqueuOrUpdate " + neighbour + " g_score: " + g_score[neighbour] + " f_score: " + f_score[neighbour]);

                    openSet.EnqueueOrUpdate(neighbour, f_score[neighbour]);
                } // foreach neighbour
            } // while openSet.Count > 0

		}
		
		private void Reconstruct_path( Dictionary<T, T> came_From, T current)
		{
			// So at this point, current IS the goal.
			// So what we want to do is walk backwards through the Came_From
			// map, until we reach the "end" of that map...which will be
			// our starting node!
			Queue<T> total_path = new Queue<T>();
			total_path.Enqueue(current); // This "final" step is the path is the goal!

			while (came_From.ContainsKey(current))
			{
				/*    Came_From is a map, where the
			*    key => value relation is real saying
			*    some_node => we_got_there_from_this_node
			*/

				current = came_From[current];
				total_path.Enqueue(current);
			}

			// At this point, total_path is a queue that is running
			// backwards from the END tile to the START tile, so let's reverse it.
			path = new Queue<T>(total_path.Reverse());
		}

		public T[] GetResult() {

			return path.ToArray();

		}

	}

}
