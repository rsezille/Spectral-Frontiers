# Complete list of Features

Can be used later to create a test sheet to check all points

## Lighting

### Board

- BoardLight lights have their intensity and range randomly changing over time
- BoardLight lights don't emit light during day, and do emit full light during night
- Shadows under entities have their opacity changing according to the SunLight intensity (day/night)

### Mission

- Can have day locked all turns (`"lighting": "day",`)
- Can have night locked all turns (`"lighting": "night",`)
- Can rotate day/night in real time (`"lighting": "auto",`)
- Can rotate day/night per turn (`"lighting": "turn",`)