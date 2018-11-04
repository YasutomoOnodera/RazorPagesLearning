using AutoMapper;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.FormattedView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.DB.DeliveryDetailService;
using static RazorPagesLearning.Service.DB.TruckAdminService;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Service
{
    public static class Startup
    {
        /// <summary>
        /// サービス層を使用するための初期化処理
        /// </summary>
        public static void startUP(IMapperConfigurationExpression cfg)
        {
            //変換設定を定義
            cfg.CreateMap<USER_ACCOUNT, HoldsPendingInformation_USER_ACCOUNT>();
            cfg.CreateMap<HoldsPendingInformation_USER_ACCOUNT,
                    Formatted_USER_ACCOUNT>();
            cfg.CreateMap<TRUCK_ADMIN, CollisionDetectableTRUCK_ADMIN>();
            cfg.CreateMap<DELIVERY_REQUEST_DETAIL, CollisionDetectable_DELIVERY_DETAIL>();
        }
    }
}
