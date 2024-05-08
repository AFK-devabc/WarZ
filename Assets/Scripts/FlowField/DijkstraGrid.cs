using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DijkstraGrid {


    public static DijkstraTile[,] generateDijkstraGrid(DijkstraTile[,] grid, Vector2Int gridSize, List<DijkstraTile> targets) {

		//flood fill out from the end point
		foreach (DijkstraTile target in targets)
		{
			DijkstraTile destination = target;
			destination.setWeight(0);
			grid[destination.getVector2d().x, destination.getVector2d().y].setWeight(0);

			List<DijkstraTile> toVisit = new List<DijkstraTile>();
			toVisit.Add(destination);//check this maybe!!!


			//for each node we need to visit, starting with the pathEnd
			for (int i = 0; i < toVisit.Count; i++)
			{

				List<DijkstraTile> neighbours = straightNeighboursOf(toVisit[i], gridSize);

				//for each neighbour of this node (only straight line neighbours, not diagonals)
				foreach (DijkstraTile neighbour in neighbours)
				{
					int neighbourWeight = grid[neighbour.getVector2d().x, neighbour.getVector2d().y].getWeight();
					if(neighbourWeight == int.MaxValue) continue;
					//We will only ever visit every node once as we are always visiting nodes in the most efficient order
					if (neighbourWeight == -1 || neighbourWeight > toVisit[i].getWeight() + 1) //if tile has not been visited or it can get a smaller weight
					{//if tile has not been visited
						neighbour.setWeight(toVisit[i].getWeight() + 1);
						grid[neighbour.getVector2d().x, neighbour.getVector2d().y].setWeight(neighbour.getWeight());
						toVisit.Add(neighbour);
					}
				}
			}
		}
        return grid;
    }

    private static List<DijkstraTile> straightNeighboursOf(DijkstraTile tile, Vector2Int gridSize) {
        List<DijkstraTile> neighbours = new List<DijkstraTile>();
        if (tile.getVector2d().x > 0) {
            neighbours.Add(new DijkstraTile(new Vector2Int (tile.getVector2d().x - 1, tile.getVector2d().y), Vector3.zero));
        }
        if (tile.getVector2d().y > 0) {
            neighbours.Add(new DijkstraTile(new Vector2Int (tile.getVector2d().x, tile.getVector2d().y - 1), Vector3.zero));
        }
        if (tile.getVector2d().x < gridSize.x - 1) {
            neighbours.Add(new DijkstraTile(new Vector2Int (tile.getVector2d().x + 1, tile.getVector2d().y), Vector3.zero));
        }
        if (tile.getVector2d().y < gridSize.y - 1) {
            neighbours.Add(new DijkstraTile(new Vector2Int (tile.getVector2d().x, tile.getVector2d().y + 1), Vector3.zero));
        }
        return neighbours;
    }
}
