# BeeGifted Gift Registry

Author: Zach Nichols

Assignment: The **Eleven Fifty Academy Red Badge final project** requires students to create a full-stack MVC application to show student knowledge of ASP.NET, n-tier architecture, MVC design pattern, and database structure. This capstone project must be deployed to Azure as a final requirement.

## Data Structure

At its core, **BeeGifted** is a gift registry website for all occasions. In addition to the standard individual-user account authentication offered by ASP.NET, this web app contains the following data tables:

- `Person` serves as an information container for the User. Each User has a Person object that contains a unique ID, a unique GUID, name information, optional date of birth, and an optional profile picture (stored as a byte array). The Person object also contains one-to-many relationships with the `Friend`, `WishList`, and `Transaction` tables.

- `Friend` serves as a one-way wrapper object for a Person `as viewed by the current User`. Each Friend object containers a unique ID, a unique GUID indicating who the current user is, an optional relationship descriptor of the Friend in question (i.e. brother, friend, roommate), and a boolean value to keep track of if a friend request is pending or already accepted. The Friend object also contains a foreign relationship to the `Person` object for the Friend in question.

- `WishList` serves as the virtual equivalent of a gift wish list. It contains a unique ID, a unique GUID indicating who the list owner is, list name, optional list/event description, and an optional event date (i.e. Christmas or July 4). It also contains foreign relationships to the list owner's `Person` object and all associated `Gift` objects (one-to-many).

- `Gift` serves as the information container for a requested gift or present. It contains a unique ID, a name, an optional description, an optional URL to a website (i.e. Amazon), an optional product image (stored as a byte array), and a quantity desired by the recipient. If the the quantity is zero, BeeGifted assumes the recipient is asking for infinite/unlimited gifts. The Gift object also contains foreign relationships to the parent `WishList` and to all associated `Transactions` (one-to-many).

- `Transaction` contains information regarding purchases/claims for a Gift. Although BeeGifted does not allow users to purchase gifts through this website, users can virtually "claim" a gift. This allows other users (but not the recipient) to see what gifts are already reserved and which have yet to be purchased. Each Transaction contains a unique ID, dates for the initial transaction and the most recent update, and the quantity purchased for that gift. It also contains foreign relationships to the parent `Gift` object and the giver's `Person` object.

As **BeeGifted** grew, I found it necessary to include additional tables for user convenience:

- `Notification` represents a message sent to a user. This includes read-only messages ("your friend request was accepted") as well as interactive messages ("John wants to be your friend. Accept or Deny?"). Each notification contains a unique ID, a notification type (read-only, friend request, etc.), and dates for the original creation as well as the most recent update. It also contains foreign relationships to the `Person` objects of both the recipient and optional sender (such as for a friend request).

- `CustomImage` contains a unique ID, unique GUID representing the user who uploaded it, and a byte array for the image itself. The custom image is primarily used to pass image data between objects and data layers.

## User Experience / Pages

- The `Home` page for the application displays the date and all upcoming events. At this time, events include birthdays and wishlist due dates for all friends. A filter field allows users to look at events in a specific time range. Depending on the type of event, the "View" button will redirect to the details page for the Friend or WishList.

- The `Friends` tab displays all currenet and pending Friends for the User. This page contains options to view a Friend's details page, edit the Friend's relationship description, or delete/remove the Friend. The included search field allows users to find any Friends with first/last names meeting the search criteria.

- The `Add Friend` button on the Friends page redirects to a similar index page displaying all BeeGifted users that are not yet Friends or pending Friends. It contains similar search functionality.

- The `Lists` tab displays a list of the User's WishLists with the ability to create, view, or delete WishLists. When viewing a Friend's WishLists, the options to create/delete are not present.

- The `WishList Details` page contains the WishList information (name, description, and date) at the top alongside a clickable image of the list owner which redirects to that owner's details page. The lower portion of this view contains a gallery view of all associated `Gifts`. The gallery can be searched by name/description or sorted by name (ascending/descending). Users can use this view to edit the list info (list owner), add/edit/view/delete gifts (list owner), or view/purchase gifts (non-owner). Non-owners can also see what quantity has already been purchased for a gift.

- Purchasing a gift brings up a `Transaction` creation view where users can specify the quantity to be purchased. Transactions can be viewed/edited/deleted in a list on the Transactions screen (view navigation bar dropdown).

- The `Notification` view can be found in the navigation bar's dropdown. Users will receive a red badge on their navigation bar to indicate they have notifications to check. Users can view and delete all read-only messages. Friend requests can be denied or accepted (brings up view to enter Friend Info).

- The `Account Info` screen lets users edit their name, date of birth, and profile picture.

- The Account Info and Gift Create/Edit screens allow users to either upload an image from their computer or receive a random image from Doodle Ipsum's API (see About page).

- The `About` screen contains information about me, **BeeGifted**, and the parties who created some of the resources.

- The `Contact Me` page contains links to my portfolio and a contact form.

## Admin Privileges

- Users can be granted the User Roles of "Admin" or "Mod". Admins have the ability to access a `Manage Users` page that lists all registered users. Admins can use this page to grant other User Roles to users or remove users from the data base (for punitive reasons). Both Admins and Mods will have access to custom messages/notifications if these tools are added in the future.

## Other Features

- If a User/Person/WishList is deleted, a notification will be generated for all affected Transactions prior to deletion. The Giver of the Transaction will receive a notification stating that the associated Gift is no longer requested, allowing them a chance to return/exchange gifts if possible. These notifications are not generated if the Giver cancels a transaction.

- Most of the Create/Edit/Detail/Delete actions throughout **BeeGifted** are handled using modal views that appear "over" the previous view rather than sending users to a new page. I chose to do this for the user experience and to prevent navigation confusion.

- Most submission forms on Create/Edit pages have data validation. This includes visual feedback (red text) telling users to enter a valid quantity, enter a sufficient number of characters, or choose a date that is in the past/future depending on the input. Forms will not submit or contact the controller until these fields are corrected.

## Resources

- [GitHub Repository](https://github.com/znichols1131/GiftRegistry)

- [Project Rubric](https://elevenfifty.instructure.com/courses/852/assignments/19082?module_item_id=80366)

- [Planning Document](https://docs.google.com/document/d/1oiI-7hdc6sIJyV5lpJlHcA9KffptduMcqSRvMt-IjNs/edit?usp=sharing)

- [Trello Board](https://trello.com/b/TYzU3VuG/red-badge-final-project)

- [Presentation](https://docs.google.com/presentation/d/1bmzjjUKzI1JH0Z4qKPU9M_g6yh0jqqjvJ20gXSBviaw/edit?usp=sharing)



