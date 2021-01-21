using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml;

namespace GameWebServer.Entities
{
    //Here be necessary generical graph implementation. 
    public class Graph <T>
    {
        protected int DEFAULT_CAPACITY = 20;
        protected int numVertex;
        protected int[,] adjMatrix;
        protected T[] vertex; 

        public Graph()
        {
            numVertex = 0;
            this.adjMatrix = new int[DEFAULT_CAPACITY, DEFAULT_CAPACITY];
            this.vertex = new T[DEFAULT_CAPACITY]; 
        }

        public int addVertex(T newVertex)
        {
            if(numVertex == vertex.Length)
            {
                expandCapacity(); 
            }
            vertex[numVertex] = newVertex; 
            for(int i = 0; i<numVertex; i++)
            {
                adjMatrix[numVertex, i] = 0;
                adjMatrix[i, numVertex] = 0; 
            }
            numVertex++;
            return numVertex; 
        }


        public void addEdge(T vertex1, T vertex2)
        {
            int index1 = getIndex(vertex1);
            int index2 = getIndex(vertex2);
            if(indexIsValid(index1) && indexIsValid(index2))
            {
                adjMatrix[index1, index2] = 1;
                adjMatrix[index2, index1] = 1; 
            }
        }

        public int getIndex(T checkVertex)
        {
            for (int i = 0; i < numVertex; i++)
                if (vertex[i].Equals(checkVertex))
                    return i;
            return -1;
        }

        public Boolean indexIsValid(int index) 
        { 
            return ((index < numVertex) && (index >= 0)); 
        }

        public void expandCapacity()
        {
            int newSize = vertex.Length * 2;
            T[] largerVertex = new T[newSize];
            int[,] largerMatrix = new int[newSize, newSize];
            for (int i = 0; i < numVertex; i++) {
                for (int j = 0; j < numVertex; j++) {
                    largerMatrix[i, j] = adjMatrix[i,j];
                }
                largerVertex[i] = vertex[i];
            }
            vertex = largerVertex;
            adjMatrix = largerMatrix; 
        }

        public T[] getVertex()
        {
            return vertex;
        }

    }

}
