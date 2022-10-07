using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
        static int rebias = 0;
		public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
		
		
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;	
        private bool randomMove;							//Boolean to determine whether or not enemy should skip a turn or move this turn.
		
		
		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();
			
			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Player").transform;

            //Setting if the enemy will move random
            System.Random rnd = new System.Random();
            if(rnd.Next() % 2 == 1)
                this.randomMove = false;
            else
                this.randomMove = false;

            //this.randomMove = false;

            //Setting playerDamage
            this.playerDamage = 40;

            //if(randomMove) Debug.Log("Esperto");
			
			//Call the start function of our base class MovingObject.
			base.Start ();
		}
		
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
            Vector3 walker = this.transform.position;
            int bias = (int)(walker.x * walker.y);
            System.Random rnd = new System.Random();
            if((rnd.Next() & bias + rebias) % 7 > 1)
                base.AttemptMove <T> (xDir, yDir);

            rebias+=1;
            //jooj
		}
		
		
		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{
            string[] map = Map.map;
            //Map.debugMap(map);
            Vector3 pos = this.transform.position;

            char[] tmp = map[(int)pos.y+1].ToCharArray();
            tmp[(int)pos.x+1] = 'A';
            map[(int)pos.y+1] = new string(tmp);


            pos = target.position;

            tmp = map[(int)pos.y+1].ToCharArray();
            tmp[(int)pos.x+1] = 'B';
            map[(int)pos.y+1] = new string(tmp);

            List<string> list = Map.toList(map);
            //Map.debugMap(map);
            //Map.debugMap(Map.map);

            /*string maplog = "";

            /*for(int i=0;i<list.Count;i++)
                maplog += list[i] + "\n";

            Debug.Log(maplog);

            /**/

			//Declare variables for X and Y axis move directions, these range from -1 to 1.
			//These values allow us to choose between the cardinal directions: up, down, left and right.
			int xDir = 0;
			int yDir = 0;



            if(!this.randomMove)
            {
                /*//If the difference in positions is approximately zero (Epsilon) do the following:
                if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
                    
                    //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                    yDir = target.position.y > transform.position.y ? 1 : -1;
                
                //If the difference in positions is not approximately zero (Epsilon) do the following:
                else
                    //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                    xDir = target.position.x > transform.position.x ? 1 : -1;*/
                //Debug.Log("Esperto\n");
                Vector3 next = AStar(list);
                xDir = (int)next.x;
                yDir = (int)next.y;
            }
            else
            {
                System.Random rnd = new System.Random();

                int direction = rnd.Next() % 4;

                switch (direction)
                {
                    case 0:
                        xDir = 1;
                        yDir = 1;
                        break;
                    case 1:
                        xDir = -1;
                        yDir = 1;
                        break;
                    case 2:
                        xDir = 1;
                        yDir = -1;
                        break;
                    case 3:
                        xDir = -1;
                        yDir = -1;
                        break;
                    default:
                        break;
                }
            }

            for(int i=0;i<Map.map.Length;i++)
            {
                Map.map[i] = Map.map[i].Replace('A',' ');
                Map.map[i] = Map.map[i].Replace('B',' ');
            }

            for(int i=0;i<map.Length;i++)
            {
                map[i] = map[i].Replace('A',' ');
                map[i] = map[i].Replace('B',' ');
            }

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMove <Player> (xDir, yDir);
			
			
		}
		
		
		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;
			
			//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
			hitPlayer.LoseFood (playerDamage);
			
			//Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");
			
			//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			//SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

        private static Vector3 AStar(List<string> map)
        {
            List<int[]> path = new List<int[]>();
            var start = new Tile();
            start.Y = map.FindIndex(x => x.Contains("A"));
            start.X = map[start.Y].IndexOf("A");


            var finish = new Tile();
            finish.Y = map.FindIndex(x => x.Contains("B"));
            finish.X = map[finish.Y].IndexOf("B");

            start.SetDistance(finish.X, finish.Y);

            var activeTiles = new List<Tile>();
            activeTiles.Add(start);
            var visitedTiles = new List<Tile>();

            while(activeTiles.Any())
            {
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                if(checkTile.X == finish.X && checkTile.Y == finish.Y)
                {
                    //We found the destination and we can be sure (Because the the OrderBy above)
                    //That it's the most low cost option. 
                    var tile = checkTile;
                    //Console.WriteLine("Retracing steps backwards...");
                    while(true)
                    {
                        
                        path.Insert(0,new int[2]);
                        path[0][0] = tile.X;
                        path[0][1] = tile.Y;
                        //Console.WriteLine($"{tile.X} : {tile.Y}");
                        if(map[tile.Y][tile.X] == '.')
                        {
                            var newMapRow = map[tile.Y].ToCharArray();
                            newMapRow[tile.X] = '*';
                            map[tile.Y] = new string(newMapRow);
                        }
                        if(map[tile.Y][tile.X] == ' ')
                        {
                            var newMapRow = map[tile.Y].ToCharArray();
                            newMapRow[tile.X] = '*';
                            map[tile.Y] = new string(newMapRow);
                        }
                        tile = tile.Parent;
                        if(tile == null)
                        {
                            //Console.WriteLine("Map looks like :");
                            /*string tmp = "";
                            map.ForEach(x => tmp += (x) + "\n");
                            Debug.Log("Jooj\n"+tmp);
                            //Console.WriteLine("Done!");

                            /*string log = "";
                            for(int i = 0; i < path.Count; i++)
                                log += (path[i][0]*2 + "-" + path[i][1]*2);/**/
                                //Debug.Log(log);
                            //return new Vector3(path[1][1],path[1][0],0f);

                            /*Vector3 me = new Vector3(path[0][1],path[0][0],0f);
                            Vector3 target = new Vector3(path[1][1],path[1][0],0f);

                            int yDir = 0, xDir = 0;*/
                            /*tmp = "";
                            for(int i=0;i<path.Count;i++)
                                tmp += path[i][1] + "-" + path[i][0] + "\n";

                            Debug.Log(tmp);*/

                            Vector3 me = new Vector3(path[0][0],path[0][1],0f);
                            Vector3 target = new Vector3(path[1][0],path[1][1],0f);

                            int xdir = 0, ydir = 0;

                            //Debug.Log(target.x +"-"+ target.y);

                            if(target.y==me.y)
                            {
                                if(target.x<me.x)
                                {
                                    xdir = -1;
                                    ydir = 0;
                                }
                                if(target.x>me.x)
                                {
                                    xdir = 1;
                                    ydir = 0;
                                }
                            }
                            else if(target.x==me.x)
                            {
                                if(target.y<me.y)
                                {
                                    xdir = 0;
                                    ydir = 1;
                                }
                                if(target.y>me.y)
                                {
                                    xdir = 0;
                                    ydir = -1;
                                }
                            }  

                            return new Vector3(xdir,ydir,0f);                          

                            // target.position.y > transform.position.y ? 1 : -1;

                            //return new Vector3(yDir,xDir,0f);
                        }
                    }
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableTiles(map, checkTile, finish);

                foreach(var walkableTile in walkableTiles)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        continue;

                    //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                    if(activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if(existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }else
                    {
                        //We've never seen this tile before so add it to the list. 
                        activeTiles.Add(walkableTile);
                    }
                }
            }
            return new Vector3(0f,0f,0f);
        }

        private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile)
        {
            var possibleTiles = new List<Tile>()
            {
                new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = map.First().Length - 1;
            var maxY = map.Count - 1;

            return possibleTiles
                    .Where(tile => tile.X >= 0 && tile.X <= maxX)
                    .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                    .Where(tile => map[tile.Y][tile.X] == ' ' || map[tile.Y][tile.X] == 'B' || map[tile.Y][tile.X] == '.')
                    .ToList();
        }
	}

    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Tile Parent { get; set; }

        //The distance is essentially the estimated distance, ignoring walls to our target. 
        //So how many tiles left and right, up and down, ignoring walls, to get there. 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }
    }
}
