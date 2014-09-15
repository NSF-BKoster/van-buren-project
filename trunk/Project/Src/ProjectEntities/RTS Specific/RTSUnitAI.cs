// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine;
using Engine.MathEx;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using ProjectCommon;

namespace ProjectEntities
{
	/// <summary>
	/// Defines the <see cref="RTSUnitAI"/> entity type.
	/// </summary>
	public class RTSUnitAIType : AIType
	{
	}

	public class RTSUnitAI : AI
	{
		//optimization
		List<Weapon> initialWeapons;

        [Browsable(false)]
        public List<Weapon> InitialWeapons
        {
            get { return initialWeapons; }
        }

		float inactiveFindTaskTimer;

		[FieldSerialize]
		Task currentTask = new Task( Task.Types.Stop );

		[FieldSerialize]
		List<Task> tasks = new List<Task>();

		///////////////////////////////////////////

		public struct Task
		{
			[FieldSerialize]
			[DefaultValue( Types.None )]
			Types type;

			[FieldSerialize]
			[DefaultValue( typeof( Vec3 ), "0 0 0" )]
			Vec3 position;

			[FieldSerialize]
			DynamicType entityType;

			[FieldSerialize]
			Dynamic entity;

			public enum Types
			{
				None,
				Stop,
				BreakableAttack,//for automatic attacks
				Hold,
				Move,
				BreakableMove,//for automatic attacks
				Attack,
				Repair,
				BreakableRepair,//for automatic repair
                SelfDestroy,

                //VB
                PreUse,
                Reload,
                PickUp,
                Chat,
                Use,
                Talk,
			}

			public Task( Types type )
			{
				this.type = type;
				this.position = new Vec3( float.NaN, float.NaN, float.NaN );
				this.entityType = null;
				this.entity = null;
			}

			public Task( Types type, Vec3 position )
			{
				this.type = type;
				this.position = position;
				this.entityType = null;
				this.entity = null;
			}

			public Task( Types type, DynamicType entityType )
			{
				this.type = type;
				this.position = new Vec3( float.NaN, float.NaN, float.NaN );
				this.entityType = entityType;
				this.entity = null;
			}

			public Task( Types type, Vec3 position, DynamicType entityType )
			{
				this.type = type;
				this.position = position;
				this.entityType = entityType;
				this.entity = null;
			}

			public Task( Types type, Dynamic entity )
			{
				this.type = type;
				this.position = new Vec3( float.NaN, float.NaN, float.NaN );
				this.entityType = null;
				this.entity = entity;
			}

			public Types Type
			{
				get { return type; }
			}

			public Vec3 Position
			{
				get { return position; }
			}

			public DynamicType EntityType
			{
				get { return entityType; }
			}

			public Dynamic Entity
			{
				get { return entity; }
			}

			public override string ToString()
			{
				string s = type.ToString();
				if( !float.IsNaN( position.X ) )
					s += ", Position: " + position.ToString();
				if( entityType != null )
					s += ", EntityType: " + entityType.Name;
				if( entity != null )
					s += ", Entity: " + entity.ToString();
				return s;
			}
		}

		///////////////////////////////////////////

		public struct UserControlPanelTask
		{
			Task task;
			bool active;
			bool enable;

			public UserControlPanelTask( Task task )
			{
				this.task = task;
				this.active = true;
				this.enable = true;
			}

			public UserControlPanelTask( Task task, bool active )
			{
				this.task = task;
				this.active = active;
				this.enable = true;
			}

			public UserControlPanelTask( Task task, bool active, bool enable )
			{
				this.task = task;
				this.active = active;
				this.enable = enable;
			}

			public Task Task
			{
				get { return task; }
			}

			public bool Active
			{
				get { return active; }
			}

			public bool Enable
			{
				get { return enable; }
			}

		}

		///////////////////////////////////////////

		RTSUnitAIType _type = null; public new RTSUnitAIType Type { get { return _type; } }

		public RTSUnitAI()
		{
			inactiveFindTaskTimer = World.Instance.Random.NextFloat() * 2;
		}

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnPostCreate(Boolean)"/>.</summary>
		protected override void OnPostCreate( bool loaded )
		{
			base.OnPostCreate( loaded );
			SubscribeToTickEvent();
		}

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnDestroy()"/>.</summary>
		protected override void OnDestroy()
		{
			ClearTaskList();
			base.OnDestroy();
		}

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnDeleteSubscribedToDeletionEvent(Entity)"/></summary>
		protected override void OnDeleteSubscribedToDeletionEvent( Entity entity )
		{
			base.OnDeleteSubscribedToDeletionEvent( entity );

			for( int n = 0; n < tasks.Count; n++ )
			{
				if( tasks[ n ].Entity == entity )
				{
					tasks.RemoveAt( n );
					n--;
				}
			}

			if( currentTask.Entity == entity )
				DoNextTask();
		}

		//protected float GetMoveObjectPriority( Unit obj )
		//{
		//	return 0;
		//}

		protected float GetAttackObjectPriority( Unit obj )
		{
			if( ControlledObject == obj )
				return 0;

			if( obj.Intellect == null )
				return 0;

			//RTSConstructor specific
			if( ControlledObject.Type.Name == "RTSConstructor" )
			{
				if( Faction == obj.Intellect.Faction )
				{
					if( obj.Health < obj.Type.HealthMax )
					{
						Vec3 distance = obj.Position - ControlledObject.Position;
						float len = distance.Length();
						return 1.0f / len + 1.0f;
					}
				}
			}
			else
			{


				if( Faction != null && obj.Intellect.Faction != null && VBFactionManager.Instance.AreEnemies(Faction as VBFactionType, obj.Intellect.Faction as VBFactionType) )
				{
					Vec3 distance = obj.Position - ControlledObject.Position;
					float len = distance.Length();
					return 1.0f / len + 1.0f;
				}
			}

			return 0;
		}

		bool InactiveFindTask()
		{
			/*if( initialWeapons.Count == 0 )
				return false;*/

			RTSUnit controlledObj = ControlledObject;
			if( controlledObj == null )
				return false;

			Dynamic newTaskAttack = null;
			float attackObjectPriority = 0;

			Vec3 controlledObjPos = controlledObj.Position;
			float radius = controlledObj./*Type.*/ViewRadius;

			Map.Instance.GetObjects( new Sphere( controlledObjPos, radius ),
				MapObjectSceneGraphGroups.UnitGroupMask, delegate( MapObject mapObject )
			{
				Unit obj = (Unit)mapObject;

				Vec3 objPos = obj.Position;

				//check distance
				Vec3 diff = objPos - controlledObjPos;
				float objDistance = diff.Length();
				if( objDistance > radius )
					return;

				float priority = GetAttackObjectPriority( obj );
				if( priority != 0 && priority > attackObjectPriority )
				{
					attackObjectPriority = priority;
					newTaskAttack = obj;
				}
			} );

            if (newTaskAttack != null)
            {
                DoTask(new Task(Task.Types.BreakableAttack, newTaskAttack), false);

                return true;
            }

			return false;
		}

		[Browsable( false )]
		public new RTSUnit ControlledObject
		{
			get { return (RTSUnit)base.ControlledObject; }
		}

		public void UpdateInitialWeapons()
		{
			RTSUnit controlledObj = ControlledObject;

			initialWeapons = new List<Weapon>();

			foreach( MapObjectAttachedObject attachedObject in controlledObj.AttachedObjects )
			{
				MapObjectAttachedMapObject attachedMapObject = attachedObject as MapObjectAttachedMapObject;
				if( attachedMapObject != null )
				{
					Weapon weapon = attachedMapObject.MapObject as Weapon;
					if( weapon != null )
					{
						initialWeapons.Add( weapon );
					}
				}
			}
		}

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnTick()"/>.</summary>
		protected override void OnTick()
		{
			base.OnTick();

			if( initialWeapons == null )
				UpdateInitialWeapons();

			TickTasks();

			if( ( currentTask.Type == Task.Types.Stop ||
				currentTask.Type == Task.Types.BreakableMove ||
				currentTask.Type == Task.Types.BreakableAttack ||
				currentTask.Type == Task.Types.BreakableRepair
				) && tasks.Count == 0 )
			{
				inactiveFindTaskTimer -= TickDelta;
				if( inactiveFindTaskTimer <= 0 )
				{
					inactiveFindTaskTimer += 1.0f;
					if( !InactiveFindTask() )
						inactiveFindTaskTimer += .5f;
				}
			}
		}


        //VB 
        protected virtual bool InTaskRange(float needDistance = 3.5f)
        {
            Vec3 targetPos;
            if (currentTask.Entity != null)
                targetPos = currentTask.Entity.Position;
            else
                targetPos = currentTask.Position;

            float distance = (ControlledObject.Position - targetPos).Length();

            if (distance != 0)
            {
                Range range = new Range(0, needDistance);

                if (currentTask.Type == Task.Types.Attack)
                {
                    MultipleActionItem itm = (ControlledObject as VBCharacter).ActiveHeldItem as MultipleActionItem;

                    if (itm != null)
                        range = itm.GetCurActionMode().UseDistanceRange;
                }

                if (distance >= range.Minimum && distance <= range.Maximum)
                {
                    //stop
                    ControlledObject.Stop();
                    RTSCharacter character = ControlledObject as RTSCharacter;
                    if (character != null)
                        character.SetLookDirection(targetPos);
                    return true;
                }
                else
                {
                    //move to target
                    ControlledObject.Move(targetPos);
                    return false;
                }
            }

            return true;
        }

		protected virtual void TickTasks()
		{
			RTSUnit controlledObj = ControlledObject;
			if( controlledObj == null )
				return;

			switch( currentTask.Type )
			{

			//Stop
			case Task.Types.Stop:
				controlledObj.Stop();
				break;

			//Move
			case Task.Types.Move:
			case Task.Types.BreakableMove:
				if( currentTask.Entity != null )
				{
					controlledObj.Move( currentTask.Entity.Position );
				}
				else
				{
					Vec3 pos = currentTask.Position;

					if( ( controlledObj.Position.ToVec2() - pos.ToVec2() ).Length() < 1.5f &&
						Math.Abs( controlledObj.Position.Z - pos.Z ) < 3.0f )
					{
						//get to
						DoNextTask();
					}
					else
						controlledObj.Move( pos );
				}
				break;


            //Reload
                case Task.Types.Reload:
                     VBWeaponItem wpn = (controlledObj as VBCharacter).ActiveHeldItem as VBWeaponItem;
                    if (wpn != null)
                    {
                        wpn.TryReload();
                        DoNextTask();
                    }
                break;

             //Pickup
                case Task.Types.PickUp:
                if (InTaskRange())
                {
                    if (currentTask.Entity as VBItem != null)
                        (controlledObj as VBCharacter).TakeItem(currentTask.Entity as VBItem);
                    DoNextTask();
                }
                break;

             //Use
                case Task.Types.Use:
                if (InTaskRange())
                {
                    InteractableObject itm = currentTask.Entity as InteractableObject;
                    if (itm != null)itm.Interact(controlledObj);
                    DoNextTask();
                }
                break;

             //Talk
                case Task.Types.Talk:
                if (InTaskRange())
                {
                    EngineConsole.Instance.ExecuteString("createWindow chat");
                    DoNextTask();
                }
                break;

            //preuse
             case Task.Types.PreUse:
                break;

			//Attack, Repair
			case Task.Types.Attack:
			case Task.Types.BreakableAttack:
			//case Task.Types.Repair:
			//case Task.Types.BreakableRepair:
				{
					/*/healed
					if( ( currentTask.Type == Task.Types.Repair ||
						currentTask.Type == Task.Types.BreakableRepair )
						&& currentTask.Entity != null )
					{
						if( currentTask.Entity.Health == currentTask.Entity.Type.HealthMax )
						{
							DoNextTask();
							break;
						}
					}*/

					float needDistance = controlledObj.Type.OptimalAttackDistanceRange.Maximum;

                    MultipleActionItem itm = (controlledObj as VBCharacter).ActiveHeldItem as MultipleActionItem;
                    if (itm != null)
                        needDistance = itm.GetCurActionMode().UseDistanceRange.Maximum;

					Vec3 targetPos;
					if( currentTask.Entity != null )
						targetPos = currentTask.Entity.Position;
					else
						targetPos = currentTask.Position;

					float distance = ( controlledObj.Position - targetPos ).Length();

					if( distance != 0 )
					{
						bool lineVisibility = false;
						{
							if( distance < needDistance )
							{
								lineVisibility = true;

								//direct line visibility check 

								Vec3 start = ControlledObject.Position;
								Ray ray = new Ray( start, targetPos - start );

								RayCastResult[] piercingResult = PhysicsWorld.Instance.RayCastPiercing(
									ray, (int)ContactGroup.CastOnlyContact );

								foreach( RayCastResult result in piercingResult )
								{
									MapObject obj = MapSystemWorld.GetMapObjectByBody( result.Shape.Body );

									if( obj != null && obj == currentTask.Entity )
										break;

									if( obj != controlledObj )
									{
										lineVisibility = false;
										break;
									}
								}
							}
						}

						//movement control 
						if( lineVisibility )
						{
							//stop
							controlledObj.Stop();

							RTSCharacter character = controlledObj as RTSCharacter;
							if( character != null )
								character.SetLookDirection( targetPos );
						}
						else
						{
							//move to target
							controlledObj.Move( targetPos );
						}

						//weapons control
						if( lineVisibility )
						{
                            (ControlledObject as VBCharacter).BaseAttack(currentTask.Entity, null);
                            DoNextTask();
                            /*
							foreach( Weapon weapon in initialWeapons )
							{
								Vec3 pos = targetPos;
								Gun gun = weapon as Gun;
								if( gun != null && currentTask.Entity != null )
									gun.GetAdvanceAttackTargetPosition( false, currentTask.Entity, false, out pos );
								weapon.SetForceFireRotationLookTo( pos );

								if( weapon.Ready )
								{
									Range range;

									range = weapon.Type.WeaponNormalMode.UseDistanceRange;
									if( distance >= range.Minimum && distance <= range.Maximum )
										weapon.TryFire( false );

									range = weapon.Type.WeaponAlternativeMode.UseDistanceRange;
									if( distance >= range.Minimum && distance <= range.Maximum )
										weapon.TryFire( true );
								}
							}*/
						}
					}

				}
				break;
			}
		}

        public virtual void ClearTaskList()
		{
			foreach( Task task in tasks )
				if( task.Entity != null )
					UnsubscribeToDeletionEvent( task.Entity );
			tasks.Clear();
		}

		protected virtual void DoTaskInternal( Task task )
		{
			if( currentTask.Entity != null )
				UnsubscribeToDeletionEvent( currentTask.Entity );

			currentTask = task;

			if( currentTask.Entity != null )
				SubscribeToDeletionEvent( currentTask.Entity );

			//Stop
			if( task.Type == Task.Types.Stop )
			{
				if( ControlledObject != null )
					ControlledObject.Stop();
			}

			//SelfDestroy
			if( task.Type == Task.Types.SelfDestroy )
			{
				ControlledObject.Die();
			}
		}

		public void DoTask( Task task, bool toQueue )
		{
			if( toQueue && currentTask.Type == Task.Types.Stop && tasks.Count == 0 )
				toQueue = false;

			if( !toQueue )
			{
				ClearTaskList();
				DoTaskInternal( task );
			}
			else
			{
				if( task.Entity != null )
					SubscribeToDeletionEvent( task.Entity );
				tasks.Add( task );
			}
		}

		protected void DoNextTask()
		{
			if( currentTask.Entity != null )
				UnsubscribeToDeletionEvent( currentTask.Entity );

			if( tasks.Count != 0 )
			{
				Task task = tasks[ 0 ];
				tasks.RemoveAt( 0 );
				if( task.Entity != null )
					UnsubscribeToDeletionEvent( task.Entity );

				DoTaskInternal( task );
			}
			else
			{
				DoTask( new Task( Task.Types.Stop ), false );
			}
		}

		public Task CurrentTask
		{
			get { return currentTask; }
		}
	}
}
