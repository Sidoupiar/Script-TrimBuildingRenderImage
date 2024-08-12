using System.Drawing;
using System.Drawing.Imaging;

namespace 裁剪建筑渲染图
{
	internal class 裁剪建筑渲染图
	{
		static void Main( string[] _ )
		{
			// ====================================================================================================
			// ========== 脚本设置 ================================================================================
			// ====================================================================================================
			// 文件夹位置
			string 原始图片位置 = "C:/Users/82763/Desktop/原始渲染图";
			string 无阴影图片位置 = "C:/Users/82763/Desktop/无阴影渲染图";
			string 输出图片位置 = "C:/Users/82763/Desktop/处理后的渲染图";
			string 缓存图片位置 = "C:/Users/82763/Desktop/缓存的渲染图";
			// 裁剪后的图片的宽度 , 高度
			int 输出宽度 = 260;
			int 输出高度 = 160;
			int 透明通道排除阈值 = 45;
			// 会自动在前缀后面添加空格
			string 输出图片前缀 = "超越雷达";
			// 背景颜色 , 色盘 0 号色 , 替换颜色应该选择色盘中存在的且和背景颜色相近的颜色
			// 阴影颜色 , 色盘 1 号色
			Color 背景颜色 = Color.FromArgb( 255 , 0 , 0 , 0 );
			Color 背景替换颜色 = Color.FromArgb( 255 , 4 , 4 , 4 );
			Color 阴影颜色 = Color.FromArgb( 255 , 0 , 0 , 196 );
			// 重复输出会作为额外的块输出
			// 重复帧之前为 _MK 动画 , 输出为 A 组 ; 之后为 _A 动画 , 输出为 C 组 ; 重复输出为 B 组
			// 帧数从 0 开始
			int MK结束帧 = 40;
			int MK结束帧额外重复输出次数 = 2;
			// 色盘的 16 - 31 号色中 [阵营红色] 的值
			List<int> 阵营红色颜色值列表 = new() { 252 , 236 , 220 , 208 , 192 , 176 , 164 , 148 , 132 , 120 , 104 , 88 , 76 , 60 , 44 , 32 };
			// ====================================================================================================
			// ========== 开始执行 ================================================================================
			// ====================================================================================================
			// 检查文件夹情况
			DirectoryInfo 原始文件夹 = new( 原始图片位置 );
			DirectoryInfo 无阴影文件夹 = new( 无阴影图片位置 );
			if( !原始文件夹.Exists || !无阴影文件夹.Exists )
			{
				Console.WriteLine( "图片文件不存在" );
				return;
			}
			DirectoryInfo 输出文件夹 = new( 输出图片位置 );
			if( !输出文件夹.Exists )
			{
				输出文件夹.Create();
			}
			// 读取渲染图
			Console.WriteLine( "读取素材中 ..." );
			List<Bitmap> 原始图片列表 = 读取图片列表( 原始文件夹 , "原始图片" , 缓存图片位置 , 输出宽度 , 输出高度 , 背景颜色 , 背景替换颜色 , 透明通道排除阈值 );
			List<Bitmap> 无阴影图片列表 = 读取图片列表( 无阴影文件夹 , "无阴影图片" , 缓存图片位置 , 输出宽度 , 输出高度 , 背景颜色 , 背景替换颜色 , 透明通道排除阈值 );
			if( 原始图片列表.Count != 无阴影图片列表.Count )
			{
				Console.WriteLine( "两个文件夹的图片数量不一致" );
				return;
			}
			Console.WriteLine( "一共读取了 " + 原始图片列表.Count + " 素材图片" );
			// 绘制实际的帧图
			Brush 背景色刷子 = new SolidBrush( 背景颜色 );
			char 分段字符 = 'A'; // 用于区分图片块 , 主要给 SB 识别的
			int 索引号 = -1; // 同样用于输出 SB 可以识别的文件结构
			int 分段数量 = MK结束帧 + 1; // 同样用于输出 SB 可以识别的文件结构
			for( int 图片索引 = 0, 图片总数 = 原始图片列表.Count ; 图片索引 < 图片总数 ; ++图片索引 )
			{
				Console.WriteLine( "处理第 " + ( 图片索引 + 1 ) + " 张图片" );
				++索引号;
				Bitmap 原始图片 = 原始图片列表[图片索引];
				Bitmap 无阴影图片 = 无阴影图片列表[图片索引];
				// 绘制无阴影帧
				Bitmap 内容图片 = new( 输出宽度 , 输出高度 , PixelFormat.Format32bppArgb );
				Graphics 内容图片笔刷 = Graphics.FromImage( 内容图片 );
				内容图片笔刷.FillRectangle( 背景色刷子 , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) );
				内容图片笔刷.DrawImage( 无阴影图片 , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) , GraphicsUnit.Pixel );
				内容图片笔刷.Flush();
				内容图片笔刷.Dispose();
				// 绘制无阴影帧图后处理透明色和替换 [阵营红色]
				for( int 像素X = 0 ; 像素X < 输出宽度 ; ++像素X )
				{
					for( int 像素Y = 0 ; 像素Y < 输出高度 ; ++像素Y )
					{
						Color 颜色 = 内容图片.GetPixel( 像素X , 像素Y );
						if( 颜色.A < 透明通道排除阈值 || 颜色.R == 背景颜色.R && 颜色.G == 背景颜色.G && 颜色.B == 背景颜色.B )
						{
							内容图片.SetPixel( 像素X , 像素Y , 背景颜色 );
						}
						else if( 需要匹配阵营红色( 颜色 ) )
						{
							int 总色差 = 255;
							int 红色 = 颜色.R;
							foreach( int 红色颜色值 in 阵营红色颜色值列表 )
							{
								int 当前色差 = Math.Abs( 颜色.R - 红色颜色值 );
								if( 总色差 > 当前色差 )
								{
									总色差 = 当前色差;
									红色 = 红色颜色值;
								}
							}
							Color 新的颜色 = Color.FromArgb( 255 , 红色 , 0 , 0 );
							内容图片.SetPixel( 像素X , 像素Y , 新的颜色 );
						}
						else
						{
							Color 新的颜色 = Color.FromArgb( 255 , 颜色.R , 颜色.G , 颜色.B );
							内容图片.SetPixel( 像素X , 像素Y , 新的颜色 );
						}
					}
				}
				// 绘制阴影帧
				Bitmap 阴影图片 = new( 输出宽度 , 输出高度 , PixelFormat.Format32bppArgb );
				Graphics 阴影图片笔刷 = Graphics.FromImage( 阴影图片 );
				阴影图片笔刷.FillRectangle( 背景色刷子 , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) );
				阴影图片笔刷.DrawImage( 原始图片 , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) , new Rectangle( 0 , 0 , 输出宽度 , 输出高度 ) , GraphicsUnit.Pixel );
				阴影图片笔刷.Flush();
				阴影图片笔刷.Dispose();
				// 同样的 , 绘制阴影帧后处理透明色
				// 这里使用了像素替换把戏 , 会根据 [无阴影帧] 中有实际内容的像素位置 , 把 [阴影帧] 中对应位置的像素替换为背景色
				for( int 像素X = 0 ; 像素X < 输出宽度 ; ++像素X )
				{
					for( int 像素Y = 0 ; 像素Y < 输出高度 ; ++像素Y )
					{
						Color 内容颜色 = 内容图片.GetPixel( 像素X , 像素Y );
						if( 内容颜色 != 背景颜色 )
						{
							阴影图片.SetPixel( 像素X , 像素Y , 背景颜色 );
							continue;
						}
						Color 颜色 = 阴影图片.GetPixel( 像素X , 像素Y );
						if( 颜色.A < 透明通道排除阈值 || 颜色.R == 背景颜色.R && 颜色.G == 背景颜色.G && 颜色.B == 背景颜色.B )
						{
							阴影图片.SetPixel( 像素X , 像素Y , 背景颜色 );
						}
						else
						{
							阴影图片.SetPixel( 像素X , 像素Y , 阴影颜色 );
						}
					}
				}
				// 输出绘制的帧图片
				内容图片.Save( 输出图片位置 + "/" + 输出图片前缀 + " " + 分段字符 + " " + 转换为字符( 索引号 ) + ".png" , ImageFormat.Png );
				阴影图片.Save( 输出图片位置 + "/" + 输出图片前缀 + " " + 分段字符 + " " + 转换为字符( 索引号 + 分段数量 ) + ".png" , ImageFormat.Png );
				if( 图片索引 == MK结束帧 )
				{
					// 重复输出 MK 结束帧
					++分段字符;
					索引号 = -1;
					for( int 数量 = 0 ; 数量 < MK结束帧额外重复输出次数 ; ++数量 )
					{
						++索引号;
						内容图片.Save( 输出图片位置 + "/" + 输出图片前缀 + " " + 分段字符 + " " + 转换为字符( 索引号 ) + ".png" , ImageFormat.Png );
						阴影图片.Save( 输出图片位置 + "/" + 输出图片前缀 + " " + 分段字符 + " " + 转换为字符( 索引号 + MK结束帧额外重复输出次数 ) + ".png" , ImageFormat.Png );
					}
					++分段字符;
					索引号 = -1;
					分段数量 = 图片总数 - MK结束帧 - 1;
				}
			}
		}

		// ====================================================================================================
		// ========== 一些工具函数 ============================================================================
		// ====================================================================================================

		static bool 需要匹配阵营红色( Color 颜色 )
		{
			return 颜色.R > ( 颜色.G + 颜色.B ) * 0.8;
		}

		static List<Bitmap> 读取图片列表( DirectoryInfo 文件夹 , string 缓存前缀 , string 缓存图片位置 , int 保留宽度 , int 保留高度 , Color 背景颜色 , Color 背景替换颜色 , int 透明通道排除阈值 )
		{
			List<Bitmap> 图片列表 = new();
			foreach( FileInfo 文件 in 文件夹.GetFiles() )
			{
				if( !文件.FullName.EndsWith( ".png" ) )
				{
					continue;
				}
				Image 读取的图片 = Image.FromFile( 文件.FullName );
				Bitmap 图片 = new( 保留宽度 , 保留高度 , PixelFormat.Format32bppArgb );
				Graphics 笔刷 = Graphics.FromImage( 图片 );
				笔刷.DrawImage( 读取的图片 , new Rectangle( 0 , 0 , 保留宽度 , 保留高度 ) , new Rectangle( ( 读取的图片.Width - 保留宽度 ) / 2 , ( 读取的图片.Height - 保留高度 ) / 2 , 保留宽度 , 保留高度 ) , GraphicsUnit.Pixel );
				笔刷.Flush();
				笔刷.Dispose();
				读取的图片.Dispose();
				for( int 像素X = 0 ; 像素X < 保留宽度 ; ++像素X )
				{
					for( int 像素Y = 0 ; 像素Y < 保留高度 ; ++像素Y )
					{
						Color 颜色 = 图片.GetPixel( 像素X , 像素Y );
						if( 颜色.A >= 透明通道排除阈值 )
						{
							if( 颜色.R == 背景颜色.R && 颜色.G == 背景颜色.G && 颜色.B == 背景颜色.B )
							{
								图片.SetPixel( 像素X , 像素Y , 背景替换颜色 );
							}
							else
							{
								Color 新的颜色 = Color.FromArgb( 255 , 颜色.R , 颜色.G , 颜色.B );
								图片.SetPixel( 像素X , 像素Y , 新的颜色 );
							}
						}
						else
						{
							图片.SetPixel( 像素X , 像素Y , 背景颜色 );
						}
					}
				}
				图片列表.Add( 图片 );
				图片.Save( 缓存图片位置 + "/" + 缓存前缀 + " " + 文件.Name , ImageFormat.Png );
			}
			return 图片列表;
		}

		static string 转换为字符( int 图片索引 )
		{
			string 图片索引字符 = 图片索引.ToString();
			while( 图片索引字符.Length < 4 )
			{
				图片索引字符 = "0" + 图片索引字符;
			}
			return 图片索引字符;
		}
	}
}