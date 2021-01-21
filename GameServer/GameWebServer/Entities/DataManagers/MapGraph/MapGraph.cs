using System;
using System.Collections.Generic;
using System.Text;

namespace GameWebServer.Entities 
{
    //Here be required more specialized operations over the Graph
    public class MapGraph<T> : Graph<T>
    {
        
        public void addEdge(T vertex1, T vertex2, int size1)
        {
            int index1 = getIndex(vertex1);
            int index2 = getIndex(vertex2);
            if (indexIsValid(index1) && indexIsValid(index2))
                adjMatrix[index1, index2] = size1;
        }

        public Boolean edgeBetweenVertexes(T vertex1, T vertex2)
        {
            int index1 = getIndex(vertex1);
            int index2 = getIndex(vertex2);
            if (indexIsValid(index1) && indexIsValid(index2))
                if (adjMatrix[index1, index2] != 0 && adjMatrix[index2, index1] != 0)
                    return true;
                else return false;
            else return false;
        }

        public List<T> connectedVertexes(T ogVertex)
        {
            List<T> connectedVertexes = new List<T>();
            int index = getIndex(ogVertex);
            if (indexIsValid(index))
                for (int i = 0; i < numVertex; i++)
                    if (adjMatrix[index, i] != 0)
                        connectedVertexes.Add(vertex[i]);

            return connectedVertexes;
        }

    }
}
