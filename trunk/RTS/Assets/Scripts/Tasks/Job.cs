using UnityEngine;
using System.Collections.Generic;

// A job is a series of tasks to be completed which may be stored in a job list in a strategic AI.
public abstract class Job {
	
	protected Player Owner { get; set; }
	
	protected Job ParentJob { get; set; }
	// Frational components of this job. All sub jobs must be complete before this job can be assigned
	protected List<Job> SubJobs { get; set; }
	// The Controllable which has been assigned to this job
	protected List<Controllable> Assignees { get; set; }
	public abstract bool Completed { get; }
	
	// isRootJob specifies whether or not this job should be added to the strategic AI's job list
	public Job(Player owner, bool isRootJob) {
		Owner = owner;
		SubJobs = new List<Job>();
		Assignees = new List<Controllable>();
		
		if(isRootJob) {
			Owner.StrategicAI.AddJob(this);
		}
	}
	
	public void AddSubJob(Job job) {
		job.ParentJob = this;
		SubJobs.Add(job);
	}
	
	// Assigns the next available sub job or this job to the given Controllable. Returns whether or not a job was assigned
	public bool AssignNextJob(Controllable assignee, bool? appendToTaskQueue) {
		if(AllSubJobsComplete) {
			// All sub jobs are complete, so try to assign this job
			if(CanTakeThisJob(assignee)) {
				AssignThisJob(assignee, appendToTaskQueue);
				return true;
			}
		} else {
			// Recurse through sub jobs and assign the first available sub job
			foreach(Job job in SubJobs) {
				if(!job.Completed && job.AssignNextJob(assignee, appendToTaskQueue)) {
					return true;
				}
			}
		}
		// Assignee is not eligible for this job or any sub jobs
		return false;
	}
	
	// Returns whether or not the given Controllable can take this job
	protected abstract bool CanTakeThisJob(Controllable assignee);
	
	// Should be overridden to add this job, in task form, to the given Controllable's task queue.
	// NOTE: If appendToTaskQueue is null, then the task should be added as an interruptAfterCurrent
	protected virtual void AssignThisJob(Controllable assignee, bool? appendToTaskQueue) {
		Assignees.Add(assignee);
	}
	
	public virtual void RemoveAssignee(Controllable assignee) {
		Assignees.Remove(assignee);
		// Attempt to automatically assign a related job if this one was completed
		if(Completed) {
			if(ParentJob != null) {
				ParentJob.AssignNextJob(assignee, null);
			} else {
				Owner.StrategicAI.JobComplete(this);
			}
		}
	}
	
	public bool AllSubJobsComplete {
		get {
			foreach(Job job in SubJobs) {
				if(!job.Completed) {
					return false;
				}
			}
			return true;
		}
	}
	
	public virtual void Cancel() {
		foreach(Job job in SubJobs) {
			job.Cancel();
		}
		Assignees.Clear();
		if(ParentJob == null) {
			Owner.StrategicAI.JobComplete(this);
		}
	}
}
