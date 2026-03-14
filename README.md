# 🍔 Kitchen Delight

A chaotic co-op game where you manage resources, build your kitchen, and cook to run a successful restaurant. 

## 🎮 Core Gameplay Mechanics
* Physics-Based Movement & Interaction: Navigate the kitchen using standard movement and a dash ability[cite: 9]. Players can pick up and drop items [cite: 12], or use physics to throw ingredients across the room [cite: 16] and kick dropped items along the floor[cite: 19].
* Dynamic Cooking System: Process ingredients using specialized stations[cite: 34]. Chop items progressively on cutting boards [cite: 35] or cook them on stoves, managing heat to prevent food from burning[cite: 37]. 
* Flexible Recipe Assembly: Assemble dishes dynamically. For example, a classic burger requires the bun to be placed first [cite: 48], but subsequent toppings (meat, cheese, lettuce) can be added in any flexible order[cite: 49].

## 🏗️ Build System
* Grid-Based Customization: Players can toggle a Build Mode to customize their restaurant layout[cite: 24]. 
* Placement Mechanics: Pick up furniture [cite: 24], rotate it [cite: 25], and validate placement using a visual "ghost" preview before snapping it to the grid[cite: 23].
* Separation of Stations: Place passive Storage units (like ingredient spawners and trash cans) [cite: 8] alongside active Processor units (like stoves and cutting counters) to optimize your workflow[cite: 7].

## 👥 Customer AI & Restaurant Loop
* Customer Management: NPCs enter the restaurant [cite: 52], find empty tables [cite: 53], and place randomized orders[cite: 54].
* Patience & Delivery: Players must deliver the correct dish directly to the customer's table [cite: 58] before their patience runs out[cite: 55].
* Cleanup: After eating, customers leave dirty dishes behind that players must clean before seating the next guest[cite: 59, 60].
* Day & Night Cycle: The game loop consists of a Morning phase for building and prep, a Rush phase where customers arrive, and a Night phase for cleaning and managing profits[cite: 62, 63].
