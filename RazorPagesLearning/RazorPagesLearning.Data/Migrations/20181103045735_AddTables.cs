using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorPagesLearning.Data.Migrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CALENDAR_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    HOLIDAY = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CALENDAR_ADMINs", x => x.HOLIDAY);
                });

            migrationBuilder.CreateTable(
                name: "DOMAINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    KIND = table.Column<string>(maxLength: 8, nullable: false),
                    JAPANESE_KIND = table.Column<string>(nullable: true),
                    CODE = table.Column<string>(maxLength: 8, nullable: false),
                    VALUE = table.Column<string>(maxLength: 128, nullable: false),
                    VALID_FLAG = table.Column<bool>(nullable: false),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOMAINs", x => new { x.KIND, x.CODE });
                });

            migrationBuilder.CreateTable(
                name: "MAIL_TEMPLATEs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MAIL_TEMPLATE_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    TEXT = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAIL_TEMPLATEs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MESSAGE_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    KIND = table.Column<int>(nullable: false),
                    MESSAGE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGE_ADMINs", x => x.KIND);
                });

            migrationBuilder.CreateTable(
                name: "PASSWORD_HISTORies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    PASSWORD = table.Column<string>(nullable: false),
                    PASSWORD_SALT = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PASSWORD_HISTORies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PASSWORD_HISTORies_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POLICies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    NAME = table.Column<int>(nullable: false),
                    VALUE = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLICies", x => x.NAME);
                });

            migrationBuilder.CreateTable(
                name: "REQUEST_HISTORies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNTID = table.Column<long>(nullable: false),
                    REQUEST_DATE = table.Column<DateTimeOffset>(nullable: true),
                    ORDER_NUMBER = table.Column<string>(maxLength: 8, nullable: true),
                    REQUEST_KIND = table.Column<string>(maxLength: 8, nullable: false),
                    DETAIL_COUNT = table.Column<int>(nullable: false),
                    WMS_STATUS = table.Column<string>(maxLength: 8, nullable: true),
                    REQUEST_COUNT = table.Column<int>(nullable: false),
                    CONFIRM_COUNT = table.Column<int>(nullable: false),
                    DELIVERY_ADMIN_ID = table.Column<long>(nullable: false),
                    SHIP_RETURN_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_RETURN_COMPANY = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_RETURN_DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_RETURN_CHARGE_NAME = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_RETURN_ZIPCODE = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_RETURN_ADDRESS = table.Column<string>(maxLength: 255, nullable: true),
                    SHIP_RETURN_TEL = table.Column<string>(maxLength: 14, nullable: true),
                    OWNER_SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: true),
                    OWNER_COMPANY = table.Column<string>(maxLength: 128, nullable: true),
                    OWNER_DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    OWNER_CHARGE = table.Column<string>(maxLength: 72, nullable: true),
                    OWNER_ZIPCODE = table.Column<string>(maxLength: 8, nullable: true),
                    OWNER_ADDRESS = table.Column<string>(maxLength: 255, nullable: true),
                    OWNER_TEL = table.Column<string>(maxLength: 14, nullable: true),
                    SPECIFIED_DATE = table.Column<DateTimeOffset>(nullable: true),
                    SPECIFIED_TIME = table.Column<string>(maxLength: 8, nullable: true),
                    FLIGHT = table.Column<string>(maxLength: 8, nullable: true),
                    COMMENT = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REQUEST_HISTORies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SHIPPER_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: false),
                    SHIPPER_NAME = table.Column<string>(maxLength: 128, nullable: false),
                    AFTERNOON_FLAG = table.Column<bool>(nullable: false),
                    PASSWORD_FLAG = table.Column<bool>(nullable: false),
                    CUSTOMER_ONLY_FLAG = table.Column<bool>(nullable: false),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHIPPER_ADMINs", x => x.SHIPPER_CODE);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_SETTINGs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MAIL_SERVER = table.Column<string>(maxLength: 512, nullable: true),
                    MAIL_PORT = table.Column<int>(nullable: false),
                    ADMIN_MAIL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_SETTINGs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TRANSPORT_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    CODE = table.Column<string>(maxLength: 128, nullable: false),
                    NAME = table.Column<string>(maxLength: 128, nullable: false),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSPORT_ADMINs", x => x.CODE);
                });

            migrationBuilder.CreateTable(
                name: "USER_DISPLAY_SETTINGs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SCREEN_ID = table.Column<int>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    PHYS_COLUMN_NAME = table.Column<string>(maxLength: 128, nullable: false),
                    LOGI_COLUMN_NAME = table.Column<string>(maxLength: 128, nullable: true),
                    CHECK_STATUS = table.Column<bool>(nullable: false),
                    DEFAULT_ORDER = table.Column<int>(nullable: false),
                    DISPLAY_ORDER = table.Column<int>(nullable: true),
                    SORT = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_DISPLAY_SETTINGs", x => new { x.SCREEN_ID, x.USER_ACCOUNT_ID, x.PHYS_COLUMN_NAME });
                    table.ForeignKey(
                        name: "FK_USER_DISPLAY_SETTINGs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_SEARCH_CONDITIONs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    CLASS1 = table.Column<bool>(nullable: false),
                    CLASS2 = table.Column<bool>(nullable: false),
                    STORAGE_MANAGE_NUMBER = table.Column<bool>(nullable: false),
                    REMARK1 = table.Column<bool>(nullable: false),
                    REMARK2 = table.Column<bool>(nullable: false),
                    NOTE = table.Column<bool>(nullable: false),
                    PRODUCT_DATE = table.Column<bool>(nullable: false),
                    STORAGE_DATE = table.Column<bool>(nullable: false),
                    PROCESSING_DATE = table.Column<bool>(nullable: false),
                    SCRAP_SCHEDULE_DATE = table.Column<bool>(nullable: false),
                    SHIPPER_RETURN = table.Column<bool>(nullable: false),
                    REGIST_DATE = table.Column<bool>(nullable: false),
                    CUSTOMER_ITEM = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_SEARCH_CONDITIONs", x => x.USER_ACCOUNT_ID);
                    table.ForeignKey(
                        name: "FK_USER_SEARCH_CONDITIONs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WK_REQUESTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    REQUEST_KIND = table.Column<string>(maxLength: 8, nullable: false),
                    REQUEST_COUNT_SUM = table.Column<int>(nullable: false),
                    DETAIL_COUNT = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_REQUESTs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WK_REQUESTs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WK_STOCKs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    STOCK_ID = table.Column<long>(nullable: false),
                    USER_ACOUNT_ID = table.Column<long>(nullable: false),
                    KIND = table.Column<string>(maxLength: 8, nullable: false),
                    STORAGE_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    CUSTOMER_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    TITLE = table.Column<string>(maxLength: 200, nullable: false),
                    SUBTITLE = table.Column<string>(maxLength: 200, nullable: true),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    SHAPE = table.Column<string>(maxLength: 20, nullable: false),
                    CLASS1 = table.Column<string>(maxLength: 8, nullable: true),
                    CLASS2 = table.Column<string>(maxLength: 8, nullable: true),
                    REMARK1 = table.Column<string>(maxLength: 72, nullable: true),
                    REMARK2 = table.Column<string>(maxLength: 72, nullable: true),
                    NOTE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_NOTE = table.Column<string>(maxLength: 2000, nullable: true),
                    PRODUCT_DATE = table.Column<string>(maxLength: 10, nullable: true),
                    SCRAP_SCHEDULE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    TIME1 = table.Column<string>(maxLength: 3, nullable: true),
                    TIME2 = table.Column<string>(maxLength: 3, nullable: true),
                    IMPORT_LINE_NUMBER = table.Column<int>(nullable: false),
                    IMPORT_ERROR_MESSAGE = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_STOCKs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WK_TABLE_PAGINATION_SETTINGs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    viewTableType = table.Column<int>(nullable: false),
                    checkAllPage = table.Column<bool>(nullable: false),
                    USER_ACCOUNTID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_TABLE_PAGINATION_SETTINGs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WK_TABLE_PAGINATION_SETTINGs_USER_ACCOUNTs_USER_ACCOUNTID",
                        column: x => x.USER_ACCOUNTID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WK_TABLE_SELECTION_SETTINGs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    viewTableType = table.Column<int>(nullable: false),
                    originalDataId = table.Column<long>(nullable: false),
                    selected = table.Column<bool>(nullable: false),
                    appendInfo = table.Column<string>(nullable: true),
                    USER_ACCOUNTID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_TABLE_SELECTION_SETTINGs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WK_TABLE_SELECTION_SETTINGs_USER_ACCOUNTs_USER_ACCOUNTID",
                        column: x => x.USER_ACCOUNTID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WK_USER_ACCOUNTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    USER_ID = table.Column<string>(maxLength: 40, nullable: false),
                    PASSWORD = table.Column<string>(nullable: true),
                    IS_NEED_PASSWORD_UPDATE = table.Column<bool>(nullable: false),
                    PERMISSION = table.Column<int>(nullable: false),
                    CURRENT_SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: true),
                    DEFAULT_DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    TRANSPORT_ADMIN_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    NAME = table.Column<string>(maxLength: 72, nullable: false),
                    KANA = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_USER_ACCOUNTs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "REQUEST_HISTORY_DETAILs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    REQUEST_HISTORY_ID = table.Column<long>(nullable: false),
                    ORDER_NUMBER = table.Column<string>(maxLength: 8, nullable: true),
                    SLIP_NUMBER = table.Column<string>(maxLength: 128, nullable: true),
                    WMS_STATUS = table.Column<string>(maxLength: 8, nullable: true),
                    REQUEST_COUNT = table.Column<int>(nullable: true),
                    CONFIRM_COUNT = table.Column<int>(nullable: true),
                    STOCK_ID = table.Column<long>(nullable: false),
                    STORAGE_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    STATUS = table.Column<string>(maxLength: 8, nullable: true),
                    CUSTOMER_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    TITLE = table.Column<string>(maxLength: 200, nullable: true),
                    SUBTITLE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: true),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    SHAPE = table.Column<string>(maxLength: 20, nullable: true),
                    CLASS1 = table.Column<string>(maxLength: 8, nullable: true),
                    CLASS2 = table.Column<string>(maxLength: 8, nullable: true),
                    REMARK1 = table.Column<string>(maxLength: 72, nullable: true),
                    REMARK2 = table.Column<string>(maxLength: 72, nullable: true),
                    NOTE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_NOTE = table.Column<string>(maxLength: 2000, nullable: true),
                    PRODUCT_DATE = table.Column<string>(maxLength: 10, nullable: true),
                    STORAGE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    PROCESSING_DATE = table.Column<DateTimeOffset>(nullable: true),
                    SCRAP_SCHEDULE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    TIME1 = table.Column<string>(maxLength: 3, nullable: true),
                    TIME2 = table.Column<string>(maxLength: 3, nullable: true),
                    STOCK_COUNT = table.Column<int>(nullable: true),
                    STORAGE_RETRIEVAL_DATE = table.Column<DateTimeOffset>(nullable: true),
                    ARRIVAL_TIME = table.Column<string>(maxLength: 128, nullable: true),
                    BARCODE = table.Column<string>(maxLength: 9, nullable: true),
                    STOCK_KIND = table.Column<string>(maxLength: 8, nullable: true),
                    UNIT = table.Column<int>(nullable: true),
                    WMS_REGIST_DATE = table.Column<DateTimeOffset>(nullable: true),
                    WMS_UPDATE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    PROJECT_NO1 = table.Column<string>(maxLength: 20, nullable: true),
                    PROJECT_NO2 = table.Column<string>(maxLength: 50, nullable: true),
                    COPYRIGHT1 = table.Column<string>(maxLength: 8, nullable: true),
                    COPYRIGHT2 = table.Column<string>(maxLength: 50, nullable: true),
                    CONTRACT1 = table.Column<string>(maxLength: 8, nullable: true),
                    CONTRACT2 = table.Column<string>(maxLength: 50, nullable: true),
                    DATA_NO1 = table.Column<string>(maxLength: 20, nullable: true),
                    DATA_NO2 = table.Column<string>(maxLength: 50, nullable: true),
                    PROCESS_JUDGE1 = table.Column<string>(maxLength: 8, nullable: true),
                    PROCESS_JUDGE2 = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REQUEST_HISTORY_DETAILs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_REQUEST_HISTORY_DETAILs_REQUEST_HISTORies_REQUEST_HISTORY_ID",
                        column: x => x.REQUEST_HISTORY_ID,
                        principalTable: "REQUEST_HISTORies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DELIVERY_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DELIVERY_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: false),
                    COMPANY = table.Column<string>(maxLength: 72, nullable: true),
                    DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    CHARGE_NAME = table.Column<string>(maxLength: 72, nullable: true),
                    ZIPCODE = table.Column<string>(maxLength: 8, nullable: true),
                    ADDRESS1 = table.Column<string>(maxLength: 72, nullable: false),
                    ADDRESS2 = table.Column<string>(maxLength: 72, nullable: true),
                    TEL = table.Column<string>(nullable: false),
                    FAX = table.Column<string>(nullable: true),
                    MAIL = table.Column<string>(maxLength: 50, nullable: true),
                    DEFAULT_FLIGHT_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    MAIL1 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL2 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL3 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL4 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL5 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL6 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL7 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL8 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL9 = table.Column<string>(maxLength: 50, nullable: true),
                    MAIL10 = table.Column<string>(maxLength: 50, nullable: true),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DELIVERY_ADMINs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DELIVERY_ADMINs_SHIPPER_ADMINs_SHIPPER_CODE",
                        column: x => x.SHIPPER_CODE,
                        principalTable: "SHIPPER_ADMINs",
                        principalColumn: "SHIPPER_CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DEPARTMENT_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: false),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: false),
                    DEPARTMENT_NAME = table.Column<string>(maxLength: 72, nullable: false),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTMENT_ADMINs", x => new { x.SHIPPER_CODE, x.DEPARTMENT_CODE });
                    table.ForeignKey(
                        name: "FK_DEPARTMENT_ADMINs_SHIPPER_ADMINs_SHIPPER_CODE",
                        column: x => x.SHIPPER_CODE,
                        principalTable: "SHIPPER_ADMINs",
                        principalColumn: "SHIPPER_CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DELIVERY_REQUESTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    TRANSPORT_ADMIN_CODE = table.Column<string>(maxLength: 128, nullable: false),
                    DELIVERY_REQUEST_NUMBER = table.Column<string>(maxLength: 128, nullable: false),
                    STATUS = table.Column<string>(maxLength: 8, nullable: false),
                    DELIVERY_DATE = table.Column<DateTimeOffset>(nullable: true),
                    CONFIRM_DATETIME = table.Column<DateTimeOffset>(nullable: true),
                    CORRECTION_DATETIME = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DELIVERY_REQUESTs", x => new { x.TRANSPORT_ADMIN_CODE, x.DELIVERY_REQUEST_NUMBER });
                    table.ForeignKey(
                        name: "FK_DELIVERY_REQUESTs_TRANSPORT_ADMINs_TRANSPORT_ADMIN_CODE",
                        column: x => x.TRANSPORT_ADMIN_CODE,
                        principalTable: "TRANSPORT_ADMINs",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRUCK_ADMINs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    TRANSPORT_ADMIN_CODE = table.Column<string>(maxLength: 128, nullable: false),
                    TRUCK_MANAGE_NUMBER = table.Column<int>(nullable: false),
                    NUMBER = table.Column<string>(maxLength: 4, nullable: true),
                    CHARGE = table.Column<string>(maxLength: 20, nullable: true),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRUCK_ADMINs", x => new { x.TRANSPORT_ADMIN_CODE, x.TRUCK_MANAGE_NUMBER });
                    table.ForeignKey(
                        name: "FK_TRUCK_ADMINs_TRANSPORT_ADMINs_TRANSPORT_ADMIN_CODE",
                        column: x => x.TRANSPORT_ADMIN_CODE,
                        principalTable: "TRANSPORT_ADMINs",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WK_USER_DEPARTMENTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: false),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_USER_DEPARTMENTs", x => new { x.USER_ACCOUNT_ID, x.SHIPPER_CODE, x.DEPARTMENT_CODE });
                    table.ForeignKey(
                        name: "FK_WK_USER_DEPARTMENTs_WK_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "WK_USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WMS_RESULT_HISTORies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    REQUEST_HISTORY_DETAIL_ID = table.Column<long>(nullable: false),
                    STOCK_ID = table.Column<long>(nullable: false),
                    STORAGE_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: false),
                    STATUS = table.Column<string>(maxLength: 8, nullable: true),
                    CUSTOMER_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    TITLE = table.Column<string>(maxLength: 200, nullable: true),
                    SUBTITLE = table.Column<string>(maxLength: 200, nullable: true),
                    SHAPE = table.Column<string>(maxLength: 20, nullable: true),
                    REMARK1 = table.Column<string>(maxLength: 72, nullable: true),
                    REMARK2 = table.Column<string>(maxLength: 72, nullable: true),
                    NOTE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_NOTE = table.Column<string>(maxLength: 2000, nullable: true),
                    PRODUCT_DATE = table.Column<string>(maxLength: 10, nullable: true),
                    STORAGE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    PROCESSING_DATE = table.Column<DateTimeOffset>(nullable: true),
                    SCRAP_SCHEDULE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    TIME1 = table.Column<string>(maxLength: 3, nullable: true),
                    TIME2 = table.Column<string>(maxLength: 3, nullable: true),
                    STORAGE_RETRIEVAL_DATE = table.Column<DateTimeOffset>(nullable: true),
                    ARRIVAL_TIME = table.Column<string>(maxLength: 128, nullable: true),
                    SLIP_NUMBER = table.Column<string>(maxLength: 128, nullable: true),
                    ORDER_NUMBER = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_RETURN_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_COMPANY = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_CHARGE_NAME = table.Column<string>(maxLength: 72, nullable: true),
                    REQUEST_COUNT = table.Column<int>(nullable: false),
                    CONFIRM_COUNT = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WMS_RESULT_HISTORies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WMS_RESULT_HISTORies_REQUEST_HISTORY_DETAILs_REQUEST_HISTORY_DETAIL_ID",
                        column: x => x.REQUEST_HISTORY_DETAIL_ID,
                        principalTable: "REQUEST_HISTORY_DETAILs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_DELIVERies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DELIVERY_ADMIN_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_DELIVERies", x => new { x.DELIVERY_ADMIN_ID, x.USER_ACCOUNT_ID });
                    table.ForeignKey(
                        name: "FK_USER_DELIVERies_DELIVERY_ADMINs_DELIVERY_ADMIN_ID",
                        column: x => x.DELIVERY_ADMIN_ID,
                        principalTable: "DELIVERY_ADMINs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USER_DELIVERies_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WK_REQUEST_DELIVERies",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WK_REQUEST_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    DELIVERY_ADMIN_ID = table.Column<long>(nullable: false),
                    DELIVERY_DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    DELIVERY_CHARGE = table.Column<string>(maxLength: 72, nullable: true),
                    SPECIFIED_DATE = table.Column<DateTimeOffset>(nullable: true),
                    SPECIFIED_TIME = table.Column<string>(maxLength: 8, nullable: true),
                    FLIGHT = table.Column<string>(maxLength: 8, nullable: true),
                    COMMENT = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_REQUEST_DELIVERies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WK_REQUEST_DELIVERies_DELIVERY_ADMINs_DELIVERY_ADMIN_ID",
                        column: x => x.DELIVERY_ADMIN_ID,
                        principalTable: "DELIVERY_ADMINs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WK_REQUEST_DELIVERies_WK_REQUESTs_WK_REQUEST_ID",
                        column: x => x.WK_REQUEST_ID,
                        principalTable: "WK_REQUESTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STOCKs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WK_STOCK_ID = table.Column<long>(nullable: true),
                    STORAGE_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    STATUS = table.Column<string>(maxLength: 8, nullable: true),
                    CUSTOMER_MANAGE_NUMBER = table.Column<string>(maxLength: 30, nullable: true),
                    TITLE = table.Column<string>(maxLength: 200, nullable: false),
                    SUBTITLE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: true),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: true),
                    SHAPE = table.Column<string>(maxLength: 20, nullable: true),
                    CLASS1 = table.Column<string>(maxLength: 8, nullable: true),
                    CLASS2 = table.Column<string>(maxLength: 8, nullable: true),
                    FILM_NO = table.Column<string>(maxLength: 7, nullable: true),
                    FILM_DETAIL_NO = table.Column<string>(maxLength: 3, nullable: true),
                    REMARK1 = table.Column<string>(maxLength: 72, nullable: true),
                    REMARK2 = table.Column<string>(maxLength: 72, nullable: true),
                    NOTE = table.Column<string>(maxLength: 200, nullable: true),
                    SHIPPER_NOTE = table.Column<string>(maxLength: 2000, nullable: true),
                    PRODUCT_DATE = table.Column<string>(maxLength: 10, nullable: true),
                    STORAGE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    PROCESSING_DATE = table.Column<DateTimeOffset>(nullable: true),
                    SCRAP_SCHEDULE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    TIME1 = table.Column<string>(maxLength: 3, nullable: true),
                    TIME2 = table.Column<string>(maxLength: 3, nullable: true),
                    STOCK_COUNT = table.Column<int>(nullable: true),
                    STORAGE_RETRIEVAL_DATE = table.Column<DateTimeOffset>(nullable: true),
                    ARRIVAL_TIME = table.Column<string>(maxLength: 128, nullable: true),
                    BARCODE = table.Column<string>(maxLength: 9, nullable: true),
                    STOCK_KIND = table.Column<string>(maxLength: 8, nullable: true),
                    UNIT = table.Column<int>(nullable: true),
                    WMS_REGIST_DATE = table.Column<DateTimeOffset>(nullable: true),
                    WMS_UPDATE_DATE = table.Column<DateTimeOffset>(nullable: true),
                    HIDE_FLAG = table.Column<bool>(nullable: false),
                    PROJECT_NO1 = table.Column<string>(maxLength: 20, nullable: true),
                    PROJECT_NO2 = table.Column<string>(maxLength: 50, nullable: true),
                    COPYRIGHT1 = table.Column<string>(maxLength: 8, nullable: true),
                    COPYRIGHT2 = table.Column<string>(maxLength: 50, nullable: true),
                    CONTRACT1 = table.Column<string>(maxLength: 8, nullable: true),
                    CONTRACT2 = table.Column<string>(maxLength: 50, nullable: true),
                    DATA_NO1 = table.Column<string>(maxLength: 20, nullable: true),
                    DATA_NO2 = table.Column<string>(maxLength: 50, nullable: true),
                    PROCESS_JUDGE1 = table.Column<string>(maxLength: 8, nullable: true),
                    PROCESS_JUDGE2 = table.Column<string>(maxLength: 50, nullable: true),
                    SLIP_NUMBER = table.Column<string>(maxLength: 128, nullable: true),
                    ORDER_NUMBER = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_RETURN_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    SHIP_RETURN_COMPANY = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_RETURN_DEPARTMENT = table.Column<string>(maxLength: 72, nullable: true),
                    SHIP_RETURN_CHARGE_NAME = table.Column<string>(maxLength: 72, nullable: true),
                    CHECK_CHASE_ID = table.Column<long>(nullable: false),
                    DELETE_FLAG = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STOCKs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_STOCKs_WK_STOCKs_WK_STOCK_ID",
                        column: x => x.WK_STOCK_ID,
                        principalTable: "WK_STOCKs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STOCKs_DEPARTMENT_ADMINs_SHIPPER_CODE_DEPARTMENT_CODE",
                        columns: x => new { x.SHIPPER_CODE, x.DEPARTMENT_CODE },
                        principalTable: "DEPARTMENT_ADMINs",
                        principalColumns: new[] { "SHIPPER_CODE", "DEPARTMENT_CODE" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USER_DEPARTMENTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    SHIPPER_CODE = table.Column<string>(maxLength: 3, nullable: false),
                    DEPARTMENT_CODE = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_DEPARTMENTs", x => new { x.USER_ACCOUNT_ID, x.SHIPPER_CODE, x.DEPARTMENT_CODE });
                    table.ForeignKey(
                        name: "FK_USER_DEPARTMENTs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_DEPARTMENTs_DEPARTMENT_ADMINs_SHIPPER_CODE_DEPARTMENT_CODE",
                        columns: x => new { x.SHIPPER_CODE, x.DEPARTMENT_CODE },
                        principalTable: "DEPARTMENT_ADMINs",
                        principalColumns: new[] { "SHIPPER_CODE", "DEPARTMENT_CODE" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DELIVERY_REQUEST_DETAILs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TRANSPORT_ADMIN_CODE = table.Column<string>(maxLength: 128, nullable: false),
                    DELIVERY_REQUEST_NUMBER = table.Column<string>(maxLength: 128, nullable: false),
                    DELIVERY_REQUEST_DETAIL_NUMBER = table.Column<string>(maxLength: 128, nullable: false),
                    TRUCK_MANAGE_NUMBER = table.Column<int>(nullable: true),
                    TRUCK_NUMBER = table.Column<string>(maxLength: 4, nullable: true),
                    TRUCK_CHARGE = table.Column<string>(maxLength: 20, nullable: true),
                    ROUTE = table.Column<int>(nullable: true),
                    COMPANY = table.Column<string>(maxLength: 128, nullable: true),
                    DELIVERY_TITLE = table.Column<string>(maxLength: 128, nullable: true),
                    DELIVERY_NOTE = table.Column<string>(maxLength: 128, nullable: true),
                    DELIVERY_AC = table.Column<string>(maxLength: 128, nullable: true),
                    CARGO_TITLE = table.Column<string>(maxLength: 128, nullable: true),
                    CARGO_NOTE = table.Column<string>(maxLength: 128, nullable: true),
                    CARGO_AC = table.Column<string>(maxLength: 128, nullable: true),
                    BARCODE = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DELIVERY_REQUEST_DETAILs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DELIVERY_REQUEST_DETAILs_TRANSPORT_ADMINs_TRANSPORT_ADMIN_CODE",
                        column: x => x.TRANSPORT_ADMIN_CODE,
                        principalTable: "TRANSPORT_ADMINs",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DELIVERY_REQUEST_DETAILs_DELIVERY_REQUESTs_TRANSPORT_ADMIN_CODE_DELIVERY_REQUEST_NUMBER",
                        columns: x => new { x.TRANSPORT_ADMIN_CODE, x.DELIVERY_REQUEST_NUMBER },
                        principalTable: "DELIVERY_REQUESTs",
                        principalColumns: new[] { "TRANSPORT_ADMIN_CODE", "DELIVERY_REQUEST_NUMBER" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REQUEST_LISTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    STOCK_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REQUEST_LISTs", x => new { x.STOCK_ID, x.USER_ACCOUNT_ID });
                    table.ForeignKey(
                        name: "FK_REQUEST_LISTs_STOCKs_STOCK_ID",
                        column: x => x.STOCK_ID,
                        principalTable: "STOCKs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REQUEST_LISTs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_ITEMs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CREATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    UPDATED_USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    STOCK_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false),
                    ITEM1_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    ITEM1_VALUE = table.Column<string>(maxLength: 50, nullable: true),
                    ITEM2_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    ITEM2_VALUE = table.Column<string>(maxLength: 50, nullable: true),
                    ITEM3_CODE = table.Column<string>(maxLength: 8, nullable: true),
                    ITEM3_VALUE = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ITEMs", x => new { x.STOCK_ID, x.USER_ACCOUNT_ID });
                    table.ForeignKey(
                        name: "FK_USER_ITEMs_STOCKs_STOCK_ID",
                        column: x => x.STOCK_ID,
                        principalTable: "STOCKs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ITEMs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WATCHLISTs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    STOCK_ID = table.Column<long>(nullable: false),
                    USER_ACCOUNT_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WATCHLISTs", x => new { x.STOCK_ID, x.USER_ACCOUNT_ID });
                    table.ForeignKey(
                        name: "FK_WATCHLISTs_STOCKs_STOCK_ID",
                        column: x => x.STOCK_ID,
                        principalTable: "STOCKs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WATCHLISTs_USER_ACCOUNTs_USER_ACCOUNT_ID",
                        column: x => x.USER_ACCOUNT_ID,
                        principalTable: "USER_ACCOUNTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WK_REQUEST_DETAILs",
                columns: table => new
                {
                    CREATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WK_REQUEST_ID = table.Column<long>(nullable: false),
                    STOCK_ID = table.Column<long>(nullable: false),
                    REQUEST_COUNT = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WK_REQUEST_DETAILs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WK_REQUEST_DETAILs_STOCKs_STOCK_ID",
                        column: x => x.STOCK_ID,
                        principalTable: "STOCKs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WK_REQUEST_DETAILs_WK_REQUESTs_WK_REQUEST_ID",
                        column: x => x.WK_REQUEST_ID,
                        principalTable: "WK_REQUESTs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACCOUNTs_TRANSPORT_ADMIN_CODE",
                table: "USER_ACCOUNTs",
                column: "TRANSPORT_ADMIN_CODE");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACCOUNTs_USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DELIVERY_ADMINs_SHIPPER_CODE",
                table: "DELIVERY_ADMINs",
                column: "SHIPPER_CODE");

            migrationBuilder.CreateIndex(
                name: "IX_DELIVERY_REQUEST_DETAILs_TRANSPORT_ADMIN_CODE_DELIVERY_REQUEST_NUMBER",
                table: "DELIVERY_REQUEST_DETAILs",
                columns: new[] { "TRANSPORT_ADMIN_CODE", "DELIVERY_REQUEST_NUMBER" });

            migrationBuilder.CreateIndex(
                name: "IX_PASSWORD_HISTORies_USER_ACCOUNT_ID",
                table: "PASSWORD_HISTORies",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_REQUEST_HISTORY_DETAILs_REQUEST_HISTORY_ID",
                table: "REQUEST_HISTORY_DETAILs",
                column: "REQUEST_HISTORY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_REQUEST_LISTs_USER_ACCOUNT_ID",
                table: "REQUEST_LISTs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_STOCKs_WK_STOCK_ID",
                table: "STOCKs",
                column: "WK_STOCK_ID",
                unique: true,
                filter: "[WK_STOCK_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_STOCKs_SHIPPER_CODE_DEPARTMENT_CODE",
                table: "STOCKs",
                columns: new[] { "SHIPPER_CODE", "DEPARTMENT_CODE" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_DELIVERies_USER_ACCOUNT_ID",
                table: "USER_DELIVERies",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_DEPARTMENTs_SHIPPER_CODE_DEPARTMENT_CODE",
                table: "USER_DEPARTMENTs",
                columns: new[] { "SHIPPER_CODE", "DEPARTMENT_CODE" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_DISPLAY_SETTINGs_USER_ACCOUNT_ID",
                table: "USER_DISPLAY_SETTINGs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ITEMs_USER_ACCOUNT_ID",
                table: "USER_ITEMs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WATCHLISTs_USER_ACCOUNT_ID",
                table: "WATCHLISTs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_REQUEST_DELIVERies_DELIVERY_ADMIN_ID",
                table: "WK_REQUEST_DELIVERies",
                column: "DELIVERY_ADMIN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_REQUEST_DELIVERies_WK_REQUEST_ID",
                table: "WK_REQUEST_DELIVERies",
                column: "WK_REQUEST_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WK_REQUEST_DETAILs_STOCK_ID",
                table: "WK_REQUEST_DETAILs",
                column: "STOCK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_REQUEST_DETAILs_WK_REQUEST_ID",
                table: "WK_REQUEST_DETAILs",
                column: "WK_REQUEST_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_REQUESTs_USER_ACCOUNT_ID",
                table: "WK_REQUESTs",
                column: "USER_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_TABLE_PAGINATION_SETTINGs_USER_ACCOUNTID",
                table: "WK_TABLE_PAGINATION_SETTINGs",
                column: "USER_ACCOUNTID");

            migrationBuilder.CreateIndex(
                name: "IX_WK_TABLE_SELECTION_SETTINGs_USER_ACCOUNTID",
                table: "WK_TABLE_SELECTION_SETTINGs",
                column: "USER_ACCOUNTID");

            migrationBuilder.CreateIndex(
                name: "IX_WMS_RESULT_HISTORies_REQUEST_HISTORY_DETAIL_ID",
                table: "WMS_RESULT_HISTORies",
                column: "REQUEST_HISTORY_DETAIL_ID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ACCOUNTs_TRANSPORT_ADMINs_TRANSPORT_ADMIN_CODE",
                table: "USER_ACCOUNTs",
                column: "TRANSPORT_ADMIN_CODE",
                principalTable: "TRANSPORT_ADMINs",
                principalColumn: "CODE",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ACCOUNTs_WK_USER_ACCOUNTs_USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs",
                column: "USER_ACCOUNT_ID",
                principalTable: "WK_USER_ACCOUNTs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USER_ACCOUNTs_TRANSPORT_ADMINs_TRANSPORT_ADMIN_CODE",
                table: "USER_ACCOUNTs");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_ACCOUNTs_WK_USER_ACCOUNTs_USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs");

            migrationBuilder.DropTable(
                name: "CALENDAR_ADMINs");

            migrationBuilder.DropTable(
                name: "DELIVERY_REQUEST_DETAILs");

            migrationBuilder.DropTable(
                name: "DOMAINs");

            migrationBuilder.DropTable(
                name: "MAIL_TEMPLATEs");

            migrationBuilder.DropTable(
                name: "MESSAGE_ADMINs");

            migrationBuilder.DropTable(
                name: "PASSWORD_HISTORies");

            migrationBuilder.DropTable(
                name: "POLICies");

            migrationBuilder.DropTable(
                name: "REQUEST_LISTs");

            migrationBuilder.DropTable(
                name: "SYSTEM_SETTINGs");

            migrationBuilder.DropTable(
                name: "TRUCK_ADMINs");

            migrationBuilder.DropTable(
                name: "USER_DELIVERies");

            migrationBuilder.DropTable(
                name: "USER_DEPARTMENTs");

            migrationBuilder.DropTable(
                name: "USER_DISPLAY_SETTINGs");

            migrationBuilder.DropTable(
                name: "USER_ITEMs");

            migrationBuilder.DropTable(
                name: "USER_SEARCH_CONDITIONs");

            migrationBuilder.DropTable(
                name: "WATCHLISTs");

            migrationBuilder.DropTable(
                name: "WK_REQUEST_DELIVERies");

            migrationBuilder.DropTable(
                name: "WK_REQUEST_DETAILs");

            migrationBuilder.DropTable(
                name: "WK_TABLE_PAGINATION_SETTINGs");

            migrationBuilder.DropTable(
                name: "WK_TABLE_SELECTION_SETTINGs");

            migrationBuilder.DropTable(
                name: "WK_USER_DEPARTMENTs");

            migrationBuilder.DropTable(
                name: "WMS_RESULT_HISTORies");

            migrationBuilder.DropTable(
                name: "DELIVERY_REQUESTs");

            migrationBuilder.DropTable(
                name: "DELIVERY_ADMINs");

            migrationBuilder.DropTable(
                name: "STOCKs");

            migrationBuilder.DropTable(
                name: "WK_REQUESTs");

            migrationBuilder.DropTable(
                name: "WK_USER_ACCOUNTs");

            migrationBuilder.DropTable(
                name: "REQUEST_HISTORY_DETAILs");

            migrationBuilder.DropTable(
                name: "TRANSPORT_ADMINs");

            migrationBuilder.DropTable(
                name: "WK_STOCKs");

            migrationBuilder.DropTable(
                name: "DEPARTMENT_ADMINs");

            migrationBuilder.DropTable(
                name: "REQUEST_HISTORies");

            migrationBuilder.DropTable(
                name: "SHIPPER_ADMINs");

            migrationBuilder.DropIndex(
                name: "IX_USER_ACCOUNTs_TRANSPORT_ADMIN_CODE",
                table: "USER_ACCOUNTs");

            migrationBuilder.DropIndex(
                name: "IX_USER_ACCOUNTs_USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs");

            migrationBuilder.DropColumn(
                name: "USER_ACCOUNT_ID",
                table: "USER_ACCOUNTs");
        }
    }
}
