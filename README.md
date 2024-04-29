**** STEPS to run the application *****
1. Download the .zip file to your local
2. Extract the zip file.
3. Open Package  Manager Console from the Tools option and run the below commands one after another : 
----> add-migration initialBuild
----> update-database
4. This would create a new SQL database "CalendarBookingDb" in your local SQL server with the "Appointments" table.

**** AREAS OF IMPROVEMENT ****
1. Add more unit test cases for the BookingRepository.cs class methods.
2. Develop a GUI for the appointment booking calendar.
