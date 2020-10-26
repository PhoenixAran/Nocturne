# Nocturne

Nocturne is a framework to help create 2D games with Monogame.  
This was created because I wanted to cherry pick the parts I liked from prime31's Nez framework (physics), and Maddy Thorson's Monocle engine (entity scene simplicity).
Some differences between Nocturne and Nez/Monocle

* Nocturne's TiledMapLoader exposes alot more to the user via the MapData class. This makes making your own tile system alot easier than Nez
* Sprite system is alot more flexible with CompositeSprites
* Doing actions during an animation is intuitive thanks to ActionFrame
* Neat custom string builder that doesn't generate garbage when adding numbers

That being said I heavily recommend you use Nez or Monocle instead of whatever this rip off is.

[Nez](https://github.com/prime31/Nez) 

[Monocle](https://bitbucket.org/MattThorson/monocle-engine) (I think the owner took down this repository)


