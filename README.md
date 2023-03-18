This project focuses on implementation of light ring and state machine player controller. 
Light ring means that player after pressing a button will create an expanding ring that will slowly reveal hidden blocks.




https://user-images.githubusercontent.com/95643408/186510085-cf34cbad-cdda-4779-adf7-773f8f57de46.mp4




The mechanism is dependent on the distance of the ring boundary to an object, and the movement of particles is shifted by position, resulting in diagonal block appearances on the right side. The effect of the reveal is achieved using shader graphs and scripts to update the values in them. The state machine enables easy maintenance of custom movements and collisions, making it easy to add new options as the number of possible actions increases.

