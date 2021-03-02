# IdFeature
Basic get entity by id functionality example

## Dependencies
  - [Entitas](https://github.com/sschmid/Entitas-CSharp) ~1.13.0
  - [EntitasGenericAddon](https://github.com/c0ffeeartc/EntitasGenericAddon)

## Installation
Copy [IdFeature](./IdFeature) folder to your project.
> Dll wouldn't work, because this example requires modification of sources to add new Scope<TScope> to Id component

## Usage
First make `Id` inherit some `Scope<T>` in [IdScopes.cs](IdFeature/IdScopes.cs)
```csharp
// IdScopes.cs
using Entitas.Generic;

namespace Custom.Scripts
{
public partial struct Id
  : Scope<Game>
  //, Scope<Cmd>
{
}

public class Game : IScope { }
public class Cmd : IScope { }
}
```

Then it becomes possible to get entity by id
```csharp
public static void IdFeatureExample()
{
  // some init code above

  Lookup_ScopeManager.RegisterAll( );
  var contexts = new Contexts();
  contexts.AddScopedContexts();

  // init id feature right after contexts.AddScopedContexts();
  contexts.IdFeature_InitAuto();

  // get ScopedContext<T>
  var contextGame = contexts.Get<Game>();
  var contextCmd = contexts.Get<Cmd>();

  // use PrimaryEntityIndex through context
  contextGame.GetEntity( nameof(Id), 32 ); // OK
  contextCmd.GetEntity( nameof(Id), 32 ); // Runtime Error or Bug

  // Generic use of PrimaryEntityIndex with Compile time correctness
  contextGame.GetSingleEntBy<Game,Id,Int32>(nameof(Id), 32); // OK
  contextCmd.GetSingleEntBy<Cmd,Id,Int32>(nameof(Id), 32); // Compilation error

  // Extension method for cleaner API
  var ent = contextGame.GetEntityById<Game,Id>( 32 );  // OK
  contextCmd.GetEntityById<Cmd,Id>( 32 );  // Compilation error
}
```

## Authors
  - [c0ffeeartc](https://github.com/c0ffeeartc)
