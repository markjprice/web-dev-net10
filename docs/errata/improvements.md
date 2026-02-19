**Improvements** (1 item)

If you have suggestions for improvements, then please [raise an issue in this repository](https://github.com/markjprice/web-dev-net9/issues) or email me at markjprice (at) gmail.com.

- [Page 127 - Exercise 2.2 – Practice exercises, Practice unit testing MVC controllers](#page-127---exercise-22--practice-exercises-practice-unit-testing-mvc-controllers)


# Page 127 - Exercise 2.2 – Practice exercises, Practice unit testing MVC controllers

> Thanks to **James P** / `pappap2159` for asking a quesiton about this in the book's Discord channel on February 16, 2026 that prompted me to add this improvement item.

"Hi - loving the books, in this book specifically in chapter 2 - exercise 2.2 unit testing the home controller ,  i found this pretty tricky and ended up mocking the logger and db (cobbling together some copilot asssitance) so i cound new up the HomeController with a logger and context - Q1) did i miss something on unit testing in this or c# .net 10 books? was quite a jump in concepts? 🙂
and Q2, was that the right way to go about it ?"

I agree, Exercise 2.2 is an unusually big jump in concepts for one of my books. I'll probably move it later in the book in the next edition. 

Here's what I wrote in the book:

Controllers are where the business logic of your website runs, so it is important to test the correctness of that logic using unit tests. Write some unit tests for `HomeController`. You can read more about how to unit test controllers at the following link:
https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/testing.

Maybe I should remove the sentence, "Write some unit tests for `HomeController`." I could treat this as a reference link, and mention that testing is covered in *Chapter 7 – Web User Interface Testing Using Playwright* and *Chapter 11 – Testing and Debugging Web Services* so the reader should probably wait until they've read those chapters before returning to controller testing.

