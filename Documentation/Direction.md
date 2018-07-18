# Direction

A character has one direction: north, south, east or west. This direction impacts directly precision and dodge chances.

Starting square can also have a direction, meaning that when a character is placed on this square, it will have the direction of the starting square.  
-> It does not have an impact for player characters, as the player is the first to play in a turn and can freely change character directions.

**Technical**

In a mission json, simply add a direction key to a starting square:

```JSON
"starting_squares": [
	{
		"posX": 1,
		"posY": 8
	},
	{
		"posX": 1,
		"posY": 7,
		"direction": "North"
	}
]
```

Values are `North`, `South`, `East`, `West`. Case doesn't matter.
