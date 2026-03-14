# 🍔 Kitchen Delight

A chaotic co-op game where you manage resources, build your kitchen, and cook to run a successful restaurant. 

## 🎮 Core Gameplay Mechanics
* Physics-Based Movement & Interaction: Navigate the kitchen using standard movement and a dash ability. Players can pick up and drop items, or use physics to throw ingredients across the room and kick dropped items along the floor.
* Dynamic Cooking System: Process ingredients using specialized stations. Chop items progressively on cutting boards or cook them on stoves, managing heat to prevent food from burning. 
* Flexible Recipe Assembly: Assemble dishes dynamically. For example, a classic burger requires the bun to be placed first, but subsequent toppings (meat, cheese, lettuce) can be added in any flexible order.

## 🏗️ Build System
* Grid-Based Customization: Players can toggle a Build Mode to customize their restaurant layout. 
* Placement Mechanics: Pick up furniture, rotate it, and validate placement using a visual "ghost" preview before snapping it to the grid.
* Separation of Stations: Place passive Storage units (like ingredient spawners and trash cans) alongside active Processor units (like stoves and cutting counters) to optimize your workflow.

## 👥 Customer AI & Restaurant Loop
* Customer Management: NPCs enter the restaurant, find empty tables, and place randomized orders.
* Patience & Delivery: Players must deliver the correct dish directly to the customer's table before their patience runs out.
* Cleanup: After eating, customers leave dirty dishes behind that players must clean before seating the next guest.
* Day & Night Cycle: The game loop consists of a Morning phase for building and prep, a Rush phase where customers arrive, and a Night phase for cleaning and managing profits.
