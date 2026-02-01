# MySchemaApp
A simple console-based application which uses real-time webscraping to save and display a users KronoX schedule. By adding a schedule with a user selected descriptive title and a schedule-Url from KronoX the application is able to serialize and save that information in a json-file. This json-file can later be deserialized to let the application asynchronously use the link to scrape the schedule of the website and display it inside the app.

Along with this basic functionality a user will also be able to do these things via the basic controls:
* Add and Save a new Schedule.
* Delete an already saved Schedule.
* Display the schedule through a customized view which removes otherwise useless columns, only retaining the useful ones.
* Display the schedule with all possible columns.
* Display the full schedule (starting at a selected date).
* Display the full schedule starting from today's date as normally displayed on KronoX.
* Select a schedule to display at startup thereby reducing the time needed to be spent in the application.
* Close down the application.

Good to know (↓):

How to select a url to send into the application:
1. Access this section of KronoX: https://schema.oru.se/avanceratschema.jsp.
2. Select a Program/Course - In my case - Kurs: IK205G-V2062V26-.
3. Press enter.
4. Press "Visa Schema".
5. When sent to the KronoX schedule you've selected the url might look like this - https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=idag&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C - paste this into the application when promted with inputting a schedule-url.
6. Save the schedule in the application.

After that you will have saved this schedule inside the application and will be able to view it whenever needed. To point it out once again this schedule is fetched through webscraping and is therefore fetched each time before the schedule is displayed. This means that the schedule will be updated each time the application tries to display the schedule which in turn means that whenever a teacher or someone else updates the schedule through their own means it will also be updated inside the app. 

TLDR: The displayed schedule will be updated each time it's displayed.
