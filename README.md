This project focuses on implementation of light ring and state machine player controller. 
Light ring means that player after pressing a button will create an expanding ring that will slowly reveal hidden blocks.


https://user-images.githubusercontent.com/95643408/186509141-42c3e725-d19a-4dd5-ab12-95df5d6e3e81.mp4

The video shows the mechanism, it is dependendent on the distance of the ring boundary to an object and also movement of ,,particles" is shifted by position, which
can be seen when blocks appear diagonally on the right side. Effect of reveal of the objects is achieved with use of shader graph and script updating values in it.

State machine itself focuses on simple implementation of custom movement and collisions organised in manner that is easier to maintain when possible actions number increases,
thus use of states enables to swiftly add new options.
