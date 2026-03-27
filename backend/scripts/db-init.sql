-- Database initialization script for CandidateTest
-- Run against SQL Server (e.g., via SSMS or sqlcmd)

IF DB_ID('CandidateTestDb') IS NULL
BEGIN
    CREATE DATABASE CandidateTestDb;
END
GO

USE CandidateTestDb;
GO

-- Users
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(250) NOT NULL,
    PasswordHash NVARCHAR(250) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);

-- Tests
IF OBJECT_ID('Tests', 'U') IS NOT NULL DROP TABLE Tests;
CREATE TABLE Tests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(250) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    TestType NVARCHAR(100) NOT NULL,
    DurationMinutes INT NOT NULL
);

-- QuestionBank master
IF OBJECT_ID('QuestionBank', 'U') IS NOT NULL DROP TABLE QuestionBank;
CREATE TABLE QuestionBank (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(20) NOT NULL,
    TestType NVARCHAR(100) NOT NULL,
    Options NVARCHAR(MAX) NULL,
    CorrectAnswer INT NULL,
    CodeSnippet NVARCHAR(MAX) NULL,
    ExpectedOutput NVARCHAR(MAX) NULL,
    TimeLimit INT NOT NULL
);

-- Questions
IF OBJECT_ID('Questions', 'U') IS NOT NULL DROP TABLE Questions;
CREATE TABLE Questions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TestId INT NOT NULL REFERENCES Tests(Id) ON DELETE CASCADE,
    QuestionBankId INT NULL REFERENCES QuestionBank(Id),
    Type NVARCHAR(20) NOT NULL,
    Prompt NVARCHAR(MAX) NOT NULL,
    Choices NVARCHAR(MAX) NULL,
    Answer NVARCHAR(MAX) NOT NULL,
    Points INT NOT NULL
);

-- Submissions
IF OBJECT_ID('Submissions', 'U') IS NOT NULL DROP TABLE Submissions;
CREATE TABLE Submissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CandidateId INT NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    TestId INT NOT NULL REFERENCES Tests(Id) ON DELETE CASCADE,
    SubmittedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Score INT NOT NULL
);

-- AnswerItems
IF OBJECT_ID('AnswerItems', 'U') IS NOT NULL DROP TABLE AnswerItems;
CREATE TABLE AnswerItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SubmissionId INT NOT NULL REFERENCES Submissions(Id) ON DELETE CASCADE,
    QuestionId INT NOT NULL REFERENCES Questions(Id),
    CandidateAnswer NVARCHAR(MAX) NOT NULL,
    Correct BIT NOT NULL
);

-- Seed data
INSERT INTO Users (Username, Email, PasswordHash, Role)
VALUES
('admin', 'admin@example.com', 'admin', 'Admin'),
('candidate', 'candidate@example.com', 'candidate', 'Candidate');

INSERT INTO Tests (Title, Description, TestType, DurationMinutes)
VALUES ('General Knowledge', 'Sample MCQ + coding test', 'General', 15);

INSERT INTO QuestionBank (QuestionText, QuestionType, TestType, Options, CorrectAnswer, CodeSnippet, ExpectedOutput, TimeLimit)
VALUES
('The capital of France?', 'MCQ', 'General', 'Paris|London|Berlin|Rome', 1, NULL, NULL, 60),
('Write a function to reverse a string.', 'Coding', 'Java', NULL, NULL, 'public String reverse(String s) { }', 'dlrow olleH', 180);

INSERT INTO Questions (TestId, QuestionBankId, Type, Prompt, Choices, Answer, Points)
VALUES
(1, 1, 'MCQ', 'The capital of France?', 'Paris|London|Berlin|Rome', 'Paris', 5),
(1, 2, 'Coding', 'Write a function to reverse a string.', NULL, 'any', 10);
