window.AhaAppUiData = {
    home: {
        recentSearches: [
            "thuê nhà",
            "căn hộ Biên Hòa",
            "điện thoại cũ",
            "tìm việc gần nhà"
        ],
        savedIds: [
            "home-bds-001",
            "home-tech-001",
            "home-xe-001"
        ],
        inboxThreads: [
            {
                title: "Nhà phố 2 mặt tiền gần chợ",
                seller: "Nguyễn Minh Khoa",
                message: "Em có thể xem nhà chiều thứ 7. Anh cần em gửi thêm ảnh mặt tiền không?",
                time: "5 phút trước",
                action: "Mở trao đổi",
                href: "/app-ui/home/detail.aspx?id=home-bds-001&ui_mode=app"
            },
            {
                title: "iPhone 14 Pro 256GB",
                seller: "Aha Tech Store",
                message: "Máy còn màu tím đen, pin 91%, có thể test trực tiếp trong hôm nay.",
                time: "17 phút trước",
                action: "Xem máy",
                href: "/app-ui/home/detail.aspx?id=home-tech-001&ui_mode=app"
            },
            {
                title: "SUV 7 chỗ máy xăng",
                seller: "Nguyễn Hoàng Nam",
                message: "Anh có thể hẹn xem xe tại Biên Hòa sau 18h, giấy tờ em chuẩn bị sẵn.",
                time: "42 phút trước",
                action: "Hẹn xem xe",
                href: "/app-ui/choxe/detail.aspx?id=xe-001&ui_mode=app"
            }
        ],
        spaces: [
            { code: "home", label: "Trang chủ", emoji: "A", href: "/app-ui/home/default.aspx" },
            { code: "batdongsan", label: "Bất động sản", emoji: "BDS", href: "/app-ui/batdongsan/default.aspx" },
            { code: "choxe", label: "Chợ xe tốt", emoji: "XE", href: "/app-ui/choxe/default.aspx" },
            { code: "dienthoai-maytinh", label: "Điện thoại máy tính", emoji: "TECH", href: "/app-ui/dienthoai-maytinh/default.aspx" },
            { code: "gianhang", label: "Gian hàng", emoji: "GH", href: "/app-ui/gianhang/default.aspx" }
        ],
        featured: [
            {
                eyebrow: "Gần đây",
                title: "Nhà nguyên căn gần khu dân cư",
                meta: "3 phòng ngủ • 92 m²",
                tone: "gold",
                href: "/app-ui/batdongsan/search.aspx?ui_mode=app&q=nha"
            },
            {
                eyebrow: "Nổi bật",
                title: "Gian hàng đang có lượt xem tăng mạnh",
                meta: "12 tin đăng mới trong ngày",
                tone: "white",
                href: "/app-ui/gianhang/default.aspx?ui_mode=app"
            }
        ],
        feedTabs: ["Dành cho bạn", "Gần bạn", "Mới nhất", "Video"],
        quickActions: [
            { label: "Danh mục", href: "/app-ui/home/categories.aspx?ui_mode=app" },
            { label: "Lịch hẹn", href: "/app-ui/home/appointments.aspx?ui_mode=app" },
            { label: "Thông báo", href: "/app-ui/home/notifications.aspx?ui_mode=app" },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        listings: [
            {
                id: "home-bds-001",
                category: "Bất động sản",
                title: "Nhà phố 2 mặt tiền gần chợ, vào ở ngay",
                price: "3,45 tỷ",
                meta: "88 m² • 3 PN",
                location: "Thu Duc, TP.HCM",
                badge: "Tin ưu tiên",
                image: "linear-gradient(135deg,#dbeafe,#bfdbfe 40%,#fef3c7)",
                summary: "Nhà đã hoàn thiện, gần chợ, trường học và cụm tiện ích dân cư."
            },
            {
                id: "home-xe-001",
                category: "Xe cộ",
                title: "SUV 7 chỗ máy xăng, bảo dưỡng đầy đủ",
                price: "615 triệu",
                meta: "2021 • 42.000 km",
                location: "Biên Hòa, Đồng Nai",
                badge: "Chính chủ",
                image: "linear-gradient(135deg,#e9d5ff,#c4b5fd 45%,#fbcfe8)",
                summary: "Xe gia đình biển số đẹp, lịch sử bảo dưỡng rõ ràng và sẵn sàng giao ngay."
            },
            {
                id: "home-tech-001",
                category: "Đồ điện tử",
                title: "iPhone 14 Pro 256GB, ngoại hình đẹp, pin cao",
                price: "19,2 triệu",
                meta: "Bảo hành 6 tháng",
                location: "Gò Vấp, TP.HCM",
                badge: "Uy tín",
                image: "linear-gradient(135deg,#d1fae5,#a7f3d0 40%,#bfdbfe)",
                summary: "Máy đẹp, full chức năng, có thể hẹn xem máy trong ngày."
            }
        ]
    },
    batdongsan: {
        quickFilters: [
            "Mua bán",
            "Cho thuê",
            "Dự án",
            "Gần đây",
            "Dưới 3 tỷ"
        ],
        quickActions: [
            { label: "Mua bán", href: "/app-ui/batdongsan/mua-ban.aspx?ui_mode=app" },
            { label: "Cho thuê", href: "/app-ui/batdongsan/cho-thue.aspx?ui_mode=app" },
            { label: "Dự án", href: "/app-ui/batdongsan/du-an.aspx?ui_mode=app" },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        propertyTypes: [
            { label: "Căn hộ", stat: "2.140 tin" },
            { label: "Nhà phố", stat: "1.285 tin" },
            { label: "Đất nền", stat: "920 tin" },
            { label: "Văn phòng", stat: "316 tin" },
            { label: "Phòng trọ", stat: "684 tin" }
        ],
        highlights: [
            {
                area: "Biên Hòa",
                summary: "Nguồn tin nhà phố và căn hộ đang tăng mạnh",
                accent: "blue"
            },
            {
                area: "Thủ Đức",
                summary: "Tin cho thuê gần trung tâm và tuyến metro",
                accent: "light"
            }
        ],
        feedTabs: ["Dành cho bạn", "Gần bạn", "Mới nhất", "Bản đồ"],
        listings: [
            {
                id: "bds-001",
                category: "Nhà phố",
                title: "Nhà 1 trệt 2 lầu hẻm xe hơi, nội thất đẹp",
                price: "4,25 tỷ",
                meta: "96 m² • 4 PN • 3 WC",
                location: "Linh Đông, Thủ Đức",
                badge: "Chính chủ",
                image: "linear-gradient(135deg,#0f172a,#1d4ed8 48%,#93c5fd)",
                summary: "Vị trí ở thật, hẻm xe hơi quay đầu, khu dân trí ổn định.",
                specs: [
                    { label: "Loại tin", value: "Nhà phố" },
                    { label: "Giao dịch", value: "Mua bán" },
                    { label: "Diện tích", value: "96 m²" },
                    { label: "Phòng ngủ", value: "4" },
                    { label: "Phòng tắm", value: "3" },
                    { label: "Pháp lý", value: "Sổ hồng riêng" }
                ],
                mapLabel: "Linh Đông, Thủ Đức, TP.HCM",
                seller: {
                    name: "Trần Đức Long",
                    role: "Môi giới chuyên khu vực Thủ Đức",
                    cta: "Gọi ngay"
                }
            },
            {
                id: "bds-002",
                category: "Căn hộ",
                title: "Căn hộ 2PN view sông, ban công rộng, nhận nhà ngay",
                price: "2,98 tỷ",
                meta: "71 m² • 2 PN • 2 WC",
                location: "An Phú, TP. Thủ Đức",
                badge: "Nổi bật",
                image: "linear-gradient(135deg,#e0f2fe,#60a5fa 42%,#1d4ed8)",
                summary: "Căn hộ view thoáng, layout dễ ở, phù hợp gia đình trẻ.",
                specs: [
                    { label: "Loại tin", value: "Căn hộ" },
                    { label: "Giao dịch", value: "Mua bán" },
                    { label: "Diện tích", value: "71 m²" },
                    { label: "Phòng ngủ", value: "2" },
                    { label: "Phòng tắm", value: "2" },
                    { label: "Hướng", value: "Đông Nam" }
                ],
                mapLabel: "An Phú, TP. Thủ Đức",
                seller: {
                    name: "Phạm Gia Hân",
                    role: "Chủ nhà",
                    cta: "Nhắn tin"
                }
            },
            {
                id: "bds-003",
                category: "Phòng trọ",
                title: "Phòng studio mới, có gác, gần khu công nghệ cao",
                price: "4,8 triệu/tháng",
                meta: "32 m² • Full nội thất",
                location: "Tăng Nhơn Phú A, Thủ Đức",
                badge: "Cho thuê nhanh",
                image: "linear-gradient(135deg,#dbeafe,#f0f9ff 48%,#7dd3fc)",
                summary: "Phòng mới, có gác và bếp riêng, hợp đồng linh hoạt.",
                specs: [
                    { label: "Loại tin", value: "Phòng trọ" },
                    { label: "Giao dịch", value: "Cho thuê" },
                    { label: "Diện tích", value: "32 m²" },
                    { label: "Nội thất", value: "Full cơ bản" },
                    { label: "Hợp đồng", value: "Linh hoạt" },
                    { label: "Tình trạng", value: "Sẵn vào ở" }
                ],
                mapLabel: "Tăng Nhơn Phú A, Thủ Đức",
                seller: {
                    name: "Nguyễn Minh Thư",
                    role: "Quản lý dãy trọ",
                    cta: "Đặt lịch xem"
                }
            },
            {
                id: "bds-004",
                category: "Đất nền",
                title: "Đất gần trường học, đường 8m, đã có sổ hồng riêng",
                price: "1,86 tỷ",
                meta: "84 m² • Hướng Đông Nam",
                location: "Long Thành, Đồng Nai",
                badge: "Pháp lý rõ",
                image: "linear-gradient(135deg,#dcfce7,#86efac 50%,#38bdf8)",
                summary: "Vị trí dễ khai thác ở hoặc đầu tư, quy hoạch dân cư hiện hữu.",
                specs: [
                    { label: "Loại tin", value: "Đất nền" },
                    { label: "Giao dịch", value: "Mua bán" },
                    { label: "Diện tích", value: "84 m²" },
                    { label: "Hướng", value: "Đông Nam" },
                    { label: "Pháp lý", value: "Sổ riêng" },
                    { label: "Đường", value: "8 m" }
                ],
                mapLabel: "Long Thành, Đồng Nai",
                seller: {
                    name: "Lê Trung Kiên",
                    role: "Chuyên viên đầu tư đất nền",
                    cta: "Xem vị trí"
                }
            }
        ]
    },
    choxe: {
        quickFilters: [
            "Dành cho bạn",
            "Gần bạn",
            "Mới nhất",
            "Xem thêm"
        ],
        quickActions: [
            { label: "Ô tô", href: "/app-ui/choxe/o-to.aspx?ui_mode=app" },
            { label: "Xe máy", href: "/app-ui/choxe/xe-may.aspx?ui_mode=app" },
            { label: "Phụ tùng", href: "/app-ui/choxe/phu-tung.aspx?ui_mode=app" },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        categories: [
            { label: "Mua bán ô tô", stat: "1.280 tin", href: "/app-ui/choxe/search.aspx?ui_mode=app&q=Mua%20b%C3%A1n%20%C3%B4%20t%C3%B4", cta: "Xem tin ô tô" },
            { label: "Đăng tin", stat: "Mở luồng seller", href: "/app-ui/gianhang/default.aspx?ui_mode=app", cta: "Đăng tin ngay" },
            { label: "Quản lý tin đăng", stat: "Theo dõi trạng thái", href: "/app-ui/gianhang/listing.aspx?ui_mode=app", cta: "Mở quản lý tin" },
            { label: "Trang công khai", stat: "Cửa hàng xe", href: "/app-ui/choxe/search.aspx?ui_mode=app&q=C%E1%BB%ADa%20h%C3%A0ng%20xe", cta: "Xem cửa hàng" },
            { label: "Theo dõi hiệu quả", stat: "Báo cáo xe", href: "/app-ui/gianhang/status.aspx?ui_mode=app", cta: "Xem báo cáo" }
        ],
        highlights: [
            {
                series: "Mua bán ô tô",
                summary: "Đi vào luồng listing ô tô và xe máy đang hiển thị trên nền tảng hiện tại.",
                accent: "dark",
                href: "/app-ui/choxe/search.aspx?ui_mode=app&q=Mua%20b%C3%A1n%20%C3%B4%20t%C3%B4"
            },
            {
                series: "Cửa hàng Aha Auto",
                summary: "Đi nhanh tới nhóm cửa hàng công khai và các tin xe nổi bật đang được quan tâm.",
                accent: "light",
                href: "/app-ui/choxe/search.aspx?ui_mode=app&q=C%E1%BB%ADa%20h%C3%A0ng%20xe"
            }
        ],
        feedTabs: ["Dành cho bạn", "Gần bạn", "Mới nhất", "Cửa hàng"],
        listings: [
            {
                id: "xe-001",
                category: "Ô tô",
                title: "SUV 7 chỗ máy xăng, lịch sử bảo dưỡng rõ, nội thất sạch",
                price: "615 triệu",
                meta: "2021 • 42.000 km • Tự động",
                location: "Biên Hòa, Đồng Nai",
                badge: "Chính chủ",
                image: "linear-gradient(135deg,#111827,#92400e 48%,#fbbf24)",
                summary: "Xe gia đình dùng kỹ, lịch bảo dưỡng rõ, phù hợp đi làm hằng ngày và đi xa cuối tuần.",
                specs: [
                    { label: "Loại xe", value: "SUV 7 chỗ" },
                    { label: "Năm sản xuất", value: "2021" },
                    { label: "Số km", value: "42.000 km" },
                    { label: "Hộp số", value: "Tự động" },
                    { label: "Nhiên liệu", value: "Xăng" },
                    { label: "Tình trạng", value: "Sẵn sàng sang tên" }
                ],
                seller: {
                    name: "Nguyễn Hoàng Nam",
                    role: "Chủ xe",
                    cta: "Xem xe"
                }
            },
            {
                id: "xe-002",
                category: "Xe máy",
                title: "Xe tay ga nữ đi, máy êm, ngoại hình đẹp, giấy tờ đủ",
                price: "32,5 triệu",
                meta: "2022 • 9.800 km • Chính chủ",
                location: "Thủ Đức, TP.HCM",
                badge: "Giá tốt",
                image: "linear-gradient(135deg,#fef3c7,#fb923c 52%,#7c2d12)",
                summary: "Xe ít đi, giữ kỹ, hợp mua sử dụng ngay trong thành phố.",
                specs: [
                    { label: "Loại xe", value: "Xe tay ga" },
                    { label: "Năm sản xuất", value: "2022" },
                    { label: "Số km", value: "9.800 km" },
                    { label: "Biển số", value: "TP.HCM" },
                    { label: "Giấy tờ", value: "Đầy đủ" },
                    { label: "Tình trạng", value: "Máy êm" }
                ],
                seller: {
                    name: "Trần Mỹ Duyên",
                    role: "Người dùng cá nhân",
                    cta: "Nhắn tin"
                }
            },
            {
                id: "xe-003",
                category: "Ô tô",
                title: "Sedan bản đủ, màu trắng, nội thất zin, chạy dịch vụ nhẹ",
                price: "458 triệu",
                meta: "2019 • 78.000 km • Số tự động",
                location: "Dĩ An, Bình Dương",
                badge: "Đang xem nhiều",
                image: "linear-gradient(135deg,#e5e7eb,#9ca3af 42%,#fbbf24)",
                summary: "Xe giữ form tốt, tiện nghi đủ dùng, phù hợp khách tìm sedan phổ thông dễ bán lại.",
                specs: [
                    { label: "Loại xe", value: "Sedan" },
                    { label: "Năm sản xuất", value: "2019" },
                    { label: "Số km", value: "78.000 km" },
                    { label: "Hộp số", value: "Tự động" },
                    { label: "Nhiên liệu", value: "Xăng" },
                    { label: "Tình trạng", value: "Nội thất zin" }
                ],
                seller: {
                    name: "Lê Quốc Bảo",
                    role: "Salon xe cũ",
                    cta: "Hẹn xem"
                }
            }
        ]
    },
    dienthoaiMayTinh: {
        quickFilters: [
            "iPhone",
            "Laptop",
            "Pin cao",
            "RAM 16GB",
            "Dưới 20 triệu"
        ],
        quickActions: [
            { label: "Điện thoại", href: "/app-ui/dienthoai-maytinh/dien-thoai.aspx?ui_mode=app" },
            { label: "Máy tính", href: "/app-ui/dienthoai-maytinh/may-tinh.aspx?ui_mode=app" },
            { label: "Phụ kiện", href: "/app-ui/dienthoai-maytinh/phu-kien.aspx?ui_mode=app" },
            { label: "Đăng tin", href: "/app-ui/auth/open-shop.aspx?ui_mode=app", action: "post" }
        ],
        categories: [
            { label: "Điện thoại", stat: "1.960 tin" },
            { label: "Laptop", stat: "1.140 tin" },
            { label: "Máy tính bảng", stat: "420 tin" },
            { label: "Phụ kiện", stat: "760 tin" },
            { label: "Gaming", stat: "318 tin" }
        ],
        highlights: [
            {
                model: "iPhone 14 Pro",
                summary: "Nhóm máy pin cao, ngoại hình đẹp đang được xem nhiều nhất hôm nay",
                accent: "deep"
            },
            {
                model: "Laptop văn phòng",
                summary: "Máy mỏng nhẹ, pin tốt, cấu hình ổn định đang giao dịch nhanh",
                accent: "soft"
            }
        ],
        feedTabs: ["Dành cho bạn", "Điện thoại", "Laptop", "Mới đăng"],
        listings: [
            {
                id: "tech-001",
                category: "Điện thoại",
                title: "iPhone 14 Pro 256GB, pin 91%, ngoại hình đẹp, đủ phụ kiện",
                price: "19,2 triệu",
                meta: "Máy đẹp • Pin 91% • Bảo hành 6 tháng",
                location: "Gò Vấp, TP.HCM",
                badge: "Uy tín",
                image: "linear-gradient(135deg,#111827,#5b21b6 48%,#a78bfa)",
                summary: "Máy đẹp, chức năng ổn định, phù hợp người cần máy cao cấp dùng lâu dài.",
                specs: [
                    { label: "Model", value: "iPhone 14 Pro" },
                    { label: "Dung lượng", value: "256GB" },
                    { label: "Pin", value: "91%" },
                    { label: "Tình trạng", value: "Máy đẹp" },
                    { label: "Bảo hành", value: "6 tháng" },
                    { label: "Phụ kiện", value: "Đủ sạc cáp" }
                ],
                seller: {
                    name: "Aha Tech Store",
                    role: "Cửa hàng thiết bị",
                    cta: "Xem máy"
                }
            },
            {
                id: "tech-002",
                category: "Laptop",
                title: "Laptop 14 inch mỏng nhẹ, chip i5, RAM 16GB, SSD 512GB",
                price: "14,8 triệu",
                meta: "Văn phòng • RAM 16GB • SSD 512GB",
                location: "Quận 10, TP.HCM",
                badge: "Cấu hình tốt",
                image: "linear-gradient(135deg,#ede9fe,#8b5cf6 46%,#1f2937)",
                summary: "Máy phù hợp học tập, làm việc và di chuyển thường xuyên, pin ổn định.",
                specs: [
                    { label: "Loại máy", value: "Laptop 14 inch" },
                    { label: "CPU", value: "Intel Core i5" },
                    { label: "RAM", value: "16GB" },
                    { label: "SSD", value: "512GB" },
                    { label: "Tình trạng", value: "Đẹp 95%" },
                    { label: "Pin", value: "Dùng 5-6 giờ" }
                ],
                seller: {
                    name: "Phạm Gia Khang",
                    role: "Người dùng cá nhân",
                    cta: "Nhắn tin"
                }
            },
            {
                id: "tech-003",
                category: "Phụ kiện",
                title: "Combo bàn phím cơ, chuột không dây và tai nghe gaming",
                price: "2,6 triệu",
                meta: "Full box • Hỗ trợ test tại chỗ",
                location: "Tân Bình, TP.HCM",
                badge: "Combo tiết kiệm",
                image: "linear-gradient(135deg,#ddd6fe,#c4b5fd 42%,#111827)",
                summary: "Bộ combo phù hợp nhu cầu chơi game và làm việc tại nhà, còn mới và hoạt động tốt.",
                specs: [
                    { label: "Danh mục", value: "Phụ kiện" },
                    { label: "Tình trạng", value: "Còn đẹp" },
                    { label: "Bảo hành", value: "Test tại chỗ" },
                    { label: "Đóng gói", value: "Full box" },
                    { label: "Sử dụng", value: "Gaming / làm việc" },
                    { label: "Mua lẻ", value: "Có hỗ trợ" }
                ],
                seller: {
                    name: "Vũ Thiên Minh",
                    role: "Người bán cá nhân",
                    cta: "Hỏi giá"
                }
            }
        ]
    },
    gianhang: {
        quickActions: [
            { label: "Quản lý tin", tone: "primary" },
            { label: "Tạo đơn", tone: "primary" },
            { label: "Chờ thanh toán", tone: "neutral" },
            { label: "Lịch hẹn", tone: "neutral" },
            { label: "Khách hàng", tone: "neutral" },
            { label: "Mở quản trị", tone: "neutral" }
        ],
        statusTabs: ["Tất cả", "Chờ duyệt", "Đang bán", "Cần sửa", "Đã ẩn"],
        metrics: [
            { label: "Tin đăng hoạt động", value: "128" },
            { label: "Khách đang trao đổi", value: "46" },
            { label: "Lịch hẹn hôm nay", value: "12" }
        ],
        listings: [
            {
                id: "gh-001",
                title: "Nhà phố căn góc - gói publish bất động sản",
                status: "Đang bán",
                statusTone: "live",
                category: "Bất động sản",
                stat: "24 lead mới",
                updatedAt: "Cập nhật 18 phút trước",
                summary: "Listing đang được projection sang BĐS và Home, cần theo dõi lead và chất lượng nội dung.",
                publishTargets: ["Gian hàng", "Home", "Bất động sản"],
                reviewNotes: ["Ảnh cover cần sáng hơn", "Cần thêm mô tả pháp lý"],
                leads: ["Khách hỏi lịch xem thứ 7", "2 khách đã xin giá cuối", "1 lead cần gọi lại lúc 19h"],
                quickActions: ["Sửa nội dung", "Đẩy tin", "Tạm ẩn tin"],
                publishHistory: ["09:20 • Publish lên Bất động sản", "09:35 • Đồng bộ lên Home", "10:10 • Đẩy lại sau khi sửa ảnh"],
                checklist: ["Kiểm tra ảnh bìa", "Gọi lại lead 19h", "Cập nhật mô tả pháp lý"]
            },
            {
                id: "gh-002",
                title: "Combo máy lọc nước mini - đang bán Home",
                status: "Cần tối ưu",
                statusTone: "warn",
                category: "Sản phẩm",
                stat: "Tỷ lệ xem 4,8%",
                updatedAt: "Cập nhật 52 phút trước",
                summary: "Cần tối ưu tiêu đề, media và giá trị chính để tăng tỷ lệ chuyển đổi.",
                publishTargets: ["Gian hàng", "Home"],
                reviewNotes: ["Thiếu video ngắn", "Nên test giá mới"],
                leads: ["1 khách hỏi bảo hành", "1 khách muốn combo 2 bộ"],
                quickActions: ["Sửa giá", "Thêm media", "Tạo ưu đãi"],
                publishHistory: ["08:45 • Lên Home", "09:00 • Điều chỉnh giá thử nghiệm"],
                checklist: ["Bổ sung video 15 giây", "Viết lại lợi ích chính"]
            },
            {
                id: "gh-003",
                title: "Dịch vụ sửa nhà nhanh - chờ duyệt nội dung",
                status: "Chờ duyệt",
                statusTone: "pending",
                category: "Dịch vụ",
                stat: "3 ghi chú cần xử lý",
                updatedAt: "Cập nhật 1 giờ trước",
                summary: "Đang trong hàng đợi duyệt, cần chỉnh sửa nội dung và hình ảnh theo note.",
                publishTargets: ["Gian hàng"],
                reviewNotes: ["Nội dung cam kết cần cụ thể hơn", "Cần thay ảnh đầu tiên"],
                leads: [],
                quickActions: ["Sửa listing", "Gửi duyệt lại", "Lưu nháp"],
                publishHistory: [],
                checklist: ["Thay ảnh đầu tiên", "Viết lại đoạn cam kết dịch vụ"]
            }
        ],
        conversations: [
            {
                name: "Trần Minh An",
                topic: "Xin lịch xem nhà thứ 7",
                state: "Cần phản hồi",
                time: "5 phút"
            },
            {
                name: "Shop Minh Chau",
                topic: "Hỏi về phí publish đa kênh",
                state: "Mới",
                time: "17 phút"
            }
        ]
    }
};
