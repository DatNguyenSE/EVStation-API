using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingPost;
using API.Entities;

namespace API.Mappers
{
    public static class ChargingPostMappers
    {
        public static ChargingPostDto ToPostDto(this ChargingPost postModel)
        {
            return new ChargingPostDto
            {
                Id = postModel.Id,
                StationId = postModel.StationId,
                Code = postModel.Code,
                Type = postModel.Type,
                PowerKW = postModel.PowerKW,
                ConnectorType = postModel.ConnectorType,
                Status = postModel.Status,
                IsWalkIn = postModel.IsWalkIn,
                QRCodeUrl = postModel.QRCode != null
                    ? $"http://localhost:5001/api/posts/{postModel.Id}/qrcode"
                    : string.Empty // BỎ CÁI NÀY VÔ THẺ <img> LÀ NÓ RA QR CODE
                                   // <img src={post.qrCodeUrl} alt="QR code" /> => VÍ DỤ THÔI NHA
            };
        }

        public static ChargingPost ToChargingPostFromCreateDto(this CreateChargingPostDto postDto)
        {
            return new ChargingPost
            {
                Type = postDto.Type,
                PowerKW = postDto.PowerKW,
                ConnectorType = postDto.ConnectorType,
                Status = postDto.Status,
                IsWalkIn = postDto.IsWalkIn
            };
        }
    }
}