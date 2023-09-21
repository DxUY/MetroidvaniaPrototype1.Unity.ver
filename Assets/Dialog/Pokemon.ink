-> main

=== main ===
Which pokemon do you choose? # speaker: Prof.Oak # portrait: Prof_Oak # layout: Left  
    + [Charmander]
        -> chosen("Charmander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
You chose {pokemon}!
-> END

=== next ===
    Well then how about a sparing match with my grandson, Gary?
->END