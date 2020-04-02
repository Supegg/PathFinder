# PathFinder

[A-Star (A*) Implementation in C# by Gustavo Franco](https://www.codeguru.com/csharp/csharp/cs_misc/designtechniques/article.php/c12527/AStar-A-Implementation-in-C-Path-Finding-PathFinder.htm)

## About the Author

***Gustavo Franco*** started with programming about 19 years ago as a teenager, from my old Commodore moving to PC/Server environment Windows/UNIX SQLServer/Oracle doing gwBasic, QBasic, Turbo Pascal, Assembler, Turbo C, BC, Clipper, Fox, SQL, C/C++, Pro*C, VB3/5/6, Java, and today loving C#. Currently working on VOIP/SIP technology. Passion for most programming languages and my son Aidan.

## About the Application

A* is a generic algorithm and there are no perfect parameters to be set. The algorithm has many things that can be set, so the best way to know the best parameters that will fit in your project is to test the different combinations.

> Note: Usually, a good A* implementation does not use a standard ArrayList or List for the open nodes. If a standard List is used, the algorithm will spend a huge amount of time searching for nodes in that list; instead, a priority queue should be used.

## This project is really useful for two reasons:

* I followed the A* implementation and I tried to implement it to run with good performance, so the algorithm itself can be easily reused in any project.

* The front end gives a full chance to experiment with many variables where you can really watch and learn how it works. You also can change the heuristic, formula, or options to analyze the best setting for your project.

* The call in the source code that does the entire job is the following:

```csharp
public List<PathFinderNode> FindPath(Point start, Point end, byte[,] grid);
```

## Algorithm Settings

### Speed

This is the rendering speed. Reducing the speed changes the look of how the algorithm opens and closes the nodes in real time.

### Diagonals

This hint will tell the algorithm whether it is allowed to process diagonals as a valid path. If this checkbox is not set, the algorithm will process just four directions instead of eight.

### Heavy Diagonals

If this check box is set, the cost of the diagonals will be bigger. That will make the algorithm avoid using diagonals.

### Punish Change Direction

If this check box is set, every time the algorithm changes direction it will have a bigger cost. The end result is that, if the path is found, it will be a smoother path without too many changes in directions and look more natural. The con is that it will take more time because it searches for extra nodes.

### Heuristic

This is a constant that will affect the estimated distance from the current position to the goal destination. A heuristic function creates this estimate of how long it will take to reach the goal state because the better the estimate, the better a short path will be found.

### Formula

This is the equation used to calculate the heuristic. Different formulas give different results. Some will be faster and others slower. The end may vary, and the formula to be used depends strongly on what the A-star algorithm will be used for.

### Use Tie Breaker

Sometimes when A* is finding the path, it will find many possible choices with the same cost and basically all of them could reach the destination. A tie breaker setting is a hint to tell the algorithm that when it has multiple choices to research, instead keeping going, to change those costs a little bit and apply a second formula to determinate what could be the best "guess" to follow. Usually, this formula increments the heuristic from the current position to the goal by multipling by a constant factor.

```csharp
Heuristic = Heuristic + Abs(CurrentX * GoalY - GoalX * CurrentY) * 0.001
```

### Search Limit

This is the limit of closed nodes to be researched before returning a "Path not found." This is a useful parameter because sometimes the grid can be too big and you need to know a path only if the goal is near from the current position or it can't spend too much time calculating the path.

### Grid Size

This parameter just affects the front end. It will change the grid size; reducing the grid size gives a chance to create a bigger test, but it will take longer to render.

### Fast PathFinder

When unchecked, the implementation used is the algorithm as it usually appears in the books. When checked, it will use my own PathFinder implementation. It requires more memory. but it is about 300 to 1500 times faster or more, depending on the map complexity (See PathFinder Optimization below).

### Show Progress

This allows seeing how the algorithm works in real time. If this box is checked, the completed time will be the calculation plus rendering time.

### Completed Time

This is the time that the algorithm took to calculate the path. To know the real value, uncheck 'Show Progress'.

### Path Finder Optimization
After I implemented the original algorithm, I got frustrated because of the amount of time it took to resolve paths, especially for bigger grids and also when there was no solution to the path. Basically. I noticed that the open and closing lists were killing the algorithm. The first optimization was to replace the open list by a sorted list and the close list by a hashtable. This made a good improvement in the calculation time, but still was not what I expected. Later, I replaced the open list from a sorted list to a priority queue; it made a change, but still was not a big difference.

The big problem was that when the grid was big (1000x1000), the number of open and close nodes in the list was huge and searching in those lists a took long time, whatever method I used. At first, I thought to use classes to keep the nodes' information, but that was going to make the garbage collection crazy between the nodes' memory allocation and releasing the memory for the nodes that were disposed. Instead of classes, I used structs and re-use them in the code. I got more improvements, but still nothing close to what StarCraft does to handle eight hundred units in real time.

My current application is like a Viso application where I need to find a path between objects. The objects are not moving, so I didn't need really a super-fast algorithm, but I needed something that can react in less than one second.

I needed to find a way to get nodes from the open list in O(1) or closer to that. I though the only way to get that was not having an open list at all. There is when I thought to use a calculation grid to store the nodes. If I knew the (X/Y) location, I could reach the node immediately O(1).

Because of this, I could get rid of the close list. I could mark a closed node directly in the calculation grid. Every node has a link to the parent node, so I did not need a close list at all. However, I could not get rid of the open list because I needed to keep getting the nodes with less total cost (F), and the second grid didn't help with that.

This time I kept using the open list (priority queue), but it was just to push and get the node with lowest cost. Priority queues are excellent to do that; they require no linear access at all.

The only cons are that the memory consumption was huge to keep a second grid, because every cell stores a node, and every node was 32 bytes long. Basically, for a map (1000x1000), I needed 32 Mb of RAM. Now I was accessing the second grid by coordinates (X/Y), so I didn't need those values in the node anymore. That reduced eight bytes, multiplied by 1,000,000. I saved 8 Mb of RAM.

Every node has a link to the parent nodes and those were int (32 bits). Because the grid can never be too big, I replaced them for ushort (16 bits). That saved another 2 bytes by node; that meant another 2 Mb of savings.

Also, I realize that the heuristic (H) is calculated for every node on the fly and it is not used any more for the algorithm, so I removed it from the node structure too. Heuristic was a int (4 bytes), so I saved another 4 Mb. Finally, I got a minimum node that took 13 bytes but, because of memory alignment, they took 16 bytes at run-time. I had to set the struct to use memory alignment for every byte.

```csharp
[StructLayout(LayoutKind.Sequential, Pack=1)]
```

Finally, I got my 13 bytes for every node. For that reason, when running the sample you will see that running FastPathFinder takes 13 Mb extra memory.

If the grid is 1024x1024 the calculation grid will take 13MB
If the grid is 512x512 the calculation grid will take 3.2MB
If the grid is 256x256 the calculation grid will take 832KB

I have to admit that took me a good amount of time to make it work because of the complexity of the A* debugging. However, I got my satisfaction when it worked. I could not believe that I got a minimun of 300 times faster for a simple grid and more than 1500 times faster for complex grids.

Still, I was inserting and removing nodes from the priority queue, which meant memory work and garbage collection coming in place, so I figure out a way to not keep the nodes inside the priority queue. Instead of pushing and popping the node, I pushed and popped the address where the node is in the calculation grid. That made me save memory and run faster. The way I was storing coordinates in the priority queue was translating a X/Y coordinate in a linear coordinate.

```csharp
Location = Y * grid width + X;
```

The problem I had there was that I had to translate back and forth a from a linear coordinate to a map coordinate. So, I changed my calculation grid from a fixed map array:

```csharp
PathFinderNode[,]
```

To a linear array:

```csharp
PathFinderNode[]
```

That made the code a lot clearer because there was no translation between map and linear coordinates. Also, I added a constraint to the size of the grid. This constrain was that the map must be a power of two in width and height. Doing that allowed me to use shifting and logical operations that are much faster than mathematical operators. Where before:

```csharp
LinearLocation = LocationY * Width + LocationX;
LocationY = LinearLocation / Width;
LocationX = LinearLocation b. LocationY;
```

Now is:

```csharp
LinearLocation = (LocationY << Log(Width, 2)) | LocationX;
LocationX = LinearLocation & Width;
LocationY = LinearLocation >> Log(Width, 2);
```

After all this, for example, when the standard PathFinder algorithm resolved the path for the map"HardToGet.astar" in 131 seconds, the optimized PathFinder got the same result in 100 milliseconds. It was a 1300 times improvement.

Another optimization is that, if the current open node is already in the open list and the current node total cost is less than the store in the list, the node should be replaced. Instead, I left the old open node in the open list and I added the new one. The old node will have a higher cost so it will be processed later, but when it is processed all nodes next to it will be closed and this node will be ignored. This is a lot faster than going to the open list and removing the old open node.

Yet another optimization is that, between path finding calls, I had to clear the calculation grid. The calculation grid store object of type PathFinderNode and every object has a field Status; this field tells whether the node is open, closed, or both. The status could be 0 = no processed, 1 = open, 2 = closed. Zeroing the grid usually took about 30 milliseconds for a 1024x1024 grid and that has to be done between pathfinding calls.

To optimize it, instead of zeroing the grid what I do is change the values for the node status between calls. Then, between calls it increments the status value by 2. So, in the first path finding search, you have:

```csharp
1 = open
2 = closed
X = not researched
```

In the second path finding search, you have:

```csharp
3 = open
4 = closed
X = not researched
```

And so on...

In this way, between path finding calls it doesn't need to clear the calculation grid. it just lets you use different values for open and closed nodes; any another value means no research.

Another small optimization was to promote all local variables to member variables; this allows you to create all variables in the heap at once instead of creating/destroying them in the stack for every pathfinder call.

In the optimization journey, basically I get rid of almost all memory allocation and sequential searches for fixed allocations and minumum search. It was like the analogy of replacing all mobile parts with solid state parts in a machine.

Still there are more optimizations that can be done, and probably I'll keep working on those. If you have some feedback about the optimization or what can be done to improve it, feel free to add a comment and I'll reply as soon as I can.

### Toolbar

The toolbar allows you to create new, load, and save test scenarios, also it allows you to insert nodes (obstacles) in the grid with different costs. By default, all nodes in the grid have a cost of "1" but can be changed to another cost, the "X" in the toolbar is cost 0 and represents an impassable node. The "start" and "end" buttons let you put the start and end positions inside the grid before running the algorithm.

The costs of the nodes are not limited to the costs defined in the toolbar; the mouse buttons can change the current cost of the nodes too. Here's what happens when you move the mouse:

| Which Button Is Pressed | Action |
| - | - | - |
| Left Button | Sets the current node to the cost specified by the active cost button in the toolbar |
| Right Button | Increments the current node cost with the cost specified by the active cost button in the toolbar |
| Left and Right | Decrements the current node cost with the cost specified by the active button in the toolbar |

### Front End

When I did the A* implementation, I took a long time to find the best parameter for my real project. Because of that, I decided to create a front-end independently from the real application to help me find those values. Because the only reason for the front end was just testing and debugging, the front end has a poor design on it; there are lots of things that can be done to make it better and run with better performance, but the objective of this article is about A* implementation and not GDI optimization. For that reason, I did not spend too much time on it.

Some time ago, I saw a pretty cool application that implements A* (I can't remember the name). It was in made in Delphi but there was no source code available. I took the main idea from there to create the front end. I didn't want to make a clone of that application. Instead, it looked really interesting and I thought that it could be nice to create something similar to that.

There is a different namespace where it contains just the algorithm and the helper classes, the "Algorithms" namespace.

> Note: The rendering in real time of the researching nodes is not persistent. Basically, the nodes are drawn directly in the window HDC (Device Context); therefore, if a portion of the window is invalidated, it won't be redraw again. If you move another window over the front end, the current path will disappear. That can be resolved by using a second grid and keeping the status of every node, but again, when I did the front end, it was was just to debug the algorithm and see how the setting affects the path search.

Probably, you will notice that setting the speed at maximum (minimum value) still doesn't have a good performance, but that's not because of the algorithm. That's because the algorithm is doing a lot of callbacks (events) to the front end to allow the rendering.

The first line inside PathFinder.cs is a #define:

```csharp
#define DEBUGON
```

If this symbol is defined, the algorithm will evaluate whether the debugging events are defined. If they are, the algorithm will make a callback (event) for every step.

When the application is used on a real project, it is strongly recommended that you remove that symbol from PathFinder.cs. Removing that symbol will greatly increase the performance of the algorithm, but if this symbol is not defined, the algorithm won't make any callback. It will return just after it found a Path or the path was not found at all.

### Conclusion

This application and code can help beginning to advanced developers explore the A* algorithm step by step. Feel free to make any comments or recommendations.
