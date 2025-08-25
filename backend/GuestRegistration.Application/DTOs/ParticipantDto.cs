﻿namespace GuestRegistration.Application.DTOs;

public class ParticipantDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}