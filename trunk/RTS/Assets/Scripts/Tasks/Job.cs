using UnityEngine;
using System.Collections.Generic;

public abstract class Job {
	
	protected Player Owner { get; set; }
	
	// Frational components of this job. All sub jobs must be complete before this job can be assigned
	protected List<List<Job>> SubJobs { get; set; }
	// The Controllable which has been assigned to this job
	protected List<Controllable> Assignees { get; set; }
	public abstract bool Completed { get; }
	
	public Job(Player owner) {
		Owner = owner;
		SubJobs = new List<List<Job>>();
		Assignees = new List<Controllable>();
		
		//TODO add all created jobs to the strategic ai of the owning player
	}
	
	public void AddSubJob(int sequence, Job job) {
		if(sequence >= SubJobs.Count) {
			SubJobs.Insert(sequence, new List<Job>());
		}
		SubJobs[sequence].Add(job);
	}
	
	// Assigns the next available sub job or this job to the given Controllable. Returns whether or not a job was assigned
	public bool AssignNextJob(Controllable assignee, bool appendToTaskQueue) {
		bool allSubJobsComplete = true;
		// Recurse through sub jobs and assign the first available job
		for(int i = 0; i < SubJobs.Count; i++) {
			if(!AllSubJobsAtSequenceComplete(i)) {
				allSubJobsComplete = false;
				foreach(Job job in SubJobs[i]) {
					if(!job.Completed && job.AssignNextJob(assignee, appendToTaskQueue)) {
						return true;
					}
				}
				// All jobs in this sequence are not complete and one was not assigned, so don't move on to the next sequence
				break;
			}
		}
		if(allSubJobsComplete) {
			// All sub jobs are complete, so try to assign this job
			if(CanTakeThisJob(assignee)) {
				AssignThisJob(assignee, appendToTaskQueue);
				return true;
			}
		}
		return false;
	}
	
	// Returns whether or not the given Controllable can take this job
	protected abstract bool CanTakeThisJob(Controllable assignee);
	
	// Adds this job, in task form, to the given Controllable's task queue
	protected virtual void AssignThisJob(Controllable assignee, bool appendToTaskQueue) {
		Assignees.Add(assignee);
		Debug.Log("job assigned to "+assignee);
	}
	
	public virtual void RemoveAssignee(Controllable assignee) {
		Assignees.Remove(assignee);
		Debug.Log("job unassigned from "+assignee);
	}
	
	public bool AllSubJobsAtSequenceComplete(int sequence) {
		foreach(Job job in SubJobs[sequence]) {
			if(!job.Completed) {
				return false;
			}
		}
		return true;
	}
	
	public bool AllSubJobsComplete() {
		for(int i = 0; i < SubJobs.Count; i++) {
			if(!AllSubJobsAtSequenceComplete(i)) {
				return false;
			}
		}
		return true;
	}
}
