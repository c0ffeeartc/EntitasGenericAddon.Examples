using System;
using System.Collections.Generic;
using System.Reflection;
using Entitas;
using Entitas.Generic;

namespace Custom.Scripts
{
public partial struct Id
		: IComponent
		, ICompData
		, IGetSingleEntByIndex<Int32>
{
	public Id( Int32 value, IContext context)
	{
		Value = value;
		Context = context;
	}

	public 					Int32 					Value;
	public 					IContext 				Context;
}

public static class IdFeature
{
	public static			void					IdFeature_InitAuto		( this Contexts contexts )
	{
		Init_Auto( contexts.All );
	}

	public static			void					Init_Auto				( List<IContext> iContextList )
	{
		Type idType					= typeof(Id);
		for ( var i = 0; i < Scopes.CompScopeTypes.Count; i++ )
		{
			Type compScopeType		= Scopes.CompScopeTypes[i];
			if ( !compScopeType.IsAssignableFrom( idType ) )
			{
				continue;
			}

			foreach ( var context in iContextList )
			{
				if ( !context.GetType().IsAssignableFrom(Scopes.ScopedContextTypes[i]) )
				{
					continue;
				}

				MethodInfo init		= typeof(IdFeature).GetMethod( nameof(IdFeature.Init_Manually), BindingFlags.Static | BindingFlags.Public );
				MethodInfo initGeneric = init.MakeGenericMethod( Scopes.IScopeTypes[i], idType );
				initGeneric.Invoke(null, new []{context});
			}
		}
	}

	public static			void					Init_Manually<TScope, TIdComp>	( ScopedContext<TScope> context )
			where TScope : IScope
			where TIdComp : struct, IComponent, Scope<TScope>
	{
		if ( Lookup<TScope, Id>.Id < 0 )
		{
			//Debug.Log( "No " + typeof(Id) +" component in " + typeof( TScope ) );
			return;
		}

		context.OnEntityCreated		+= ( context1, entity ) => 
			{
				// TODO: comment why Replace and not Add
				var componentLookupId = Lookup<TScope,Id>.Id;
				var component			= entity.CreateComponent<StructComponent<Id>>( componentLookupId );
				component.Data			= new Id( entity.creationIndex, context1 );
				entity.ReplaceComponent(componentLookupId, component);
			};

		AddEntityIndex<TScope,TIdComp>( context );
	}

	public static			void					AddEntityIndex<TScope, TComp>( ScopedContext<TScope> context )
			where TScope : IScope
			where TComp : IComponent, Scope<TScope>
	{
		context.AddEntityIndex(
			nameof(Id)
			, context.GetGroup( Matcher<TScope, TComp>.I )
			, ( e, c ) => ( (StructComponent<Id>)c ).Data.Value );
	}

	public static			Entity<TScope>			GetEntityById<TScope, TComp>( this ScopedContext<TScope> context, Int32 id )
			where TScope : IScope
			where TComp : Scope<TScope>
	{
		return context.GetEntity( nameof(Id), id );
	}
}
}
