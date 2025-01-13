# CareNavigatorSparrow
Managing the state of patient care using eventsourcing of data feeds
Application shall track the state of patients, nurses, and doctors within a small or large practive using event sources.  No User input is necessary as the data comes from the event sources.

# Event sources
* ADT using faker/bogus
* RTLS random over a fixed set of locations, using a fixed set of tags
* timeclock system

# Entities
1. Patient
2. Caregiver
    1. Nurse
    2. Doctor
3. Room
    1. Bed
4. RTLS tag 

# Events
event ncessary for he system to operate
+ Patient
    + patient assigned tag (RTLS)
    + patient unassigned tag (RTLS)
    + Patient registered to bed (ADT)
    + Patient entered room (RTLS)
    + Patient exited room (RTLS)
    + Patient unregistered from bed
+ Nurse
    + nurse onduty (timeclock)
    + nurse offduty (timeclock)
    + nurse entered room (RTLS)
    + nurse exited room (RTLS)
    + nurse assigned tag (RTLS)
    + nurse unassigned tag (RTLS)

# Care Events
events that have value to the business
+ nurse saw patient for the first time
+ nurse rounded on patient to check on them
+ nurse saw patient for the last time


# Architecture
<insert C4 diagram>

# Event Modeling
